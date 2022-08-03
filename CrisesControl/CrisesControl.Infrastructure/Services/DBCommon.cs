﻿using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using CrisesControl.Core.Exceptions.NotFound;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CrisesControl.Core.Exceptions.InvalidOperation;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using CrisesControl.Core.CompanyParameters;
using Microsoft.EntityFrameworkCore;
using CrisesControl.Infrastructure.Services;
using log4net;
using Microsoft.Extensions.Logging;
using System.Reflection;
using CrisesControl.Core.Incidents;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;
using CrisesControl.Core.Locations;
using System.Net;
using System.Xml.Linq;
using Location = CrisesControl.Core.Locations.Location;
using CrisesControl.Infrastructure.Services.Jobs;

namespace CrisesControl.Api.Application.Helpers
{
    public class DBCommon
    {
        private readonly CrisesControlContext _context;
        private int userId;
        private int companyId;
        private readonly string timeZoneId = "GMT Standard Time";
        private readonly IHttpContextAccessor _httpContextAccessor;
        ILog Logger = LogManager.GetLogger(System.Environment.MachineName);


        public DBCommon(CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            companyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        }

        public StringBuilder ReadHtmlFile(string fileCode, string source, int companyId, out string subject, string provider = "AWSSES")
        {
            StringBuilder htmlContent = new StringBuilder();
            string line;
            subject = "";
            try
            {
                if (source == "FILE")
                {
                    using (StreamReader htmlReader = new System.IO.StreamReader(fileCode))
                    {
                        while ((line = htmlReader.ReadLine()) != null)
                        {
                            htmlContent.Append(line);
                        }
                    }
                }
                else
                {
                    var content = (from MSG in _context.Set<EmailTemplate>()
                                   where MSG.Code == fileCode
                                   where MSG.CompanyId == 0
                                   select MSG)
                                   .Union(from MSG in _context.Set<EmailTemplate>()
                                          where MSG.Code == fileCode && MSG.CompanyId == companyId
                                          select MSG)
                                          .OrderByDescending(o => o.CompanyId).FirstOrDefault();
                    if (content != null)
                    {
                        subject = content.EmailSubject;

                        var head = (from MSG in _context.Set<EmailTemplate>()
                                    where MSG.Code == "DOCHEAD"
                                    select MSG).FirstOrDefault();
                        if (head != null)
                        {
                            htmlContent.AppendLine(head.HtmlData.ToString());
                        }

                        htmlContent.Append(content.HtmlData.ToString());

                        if (provider.ToUpper() != "OFFICE365")
                        {
                            var desc = (from MSG in _context.Set<EmailTemplate>()
                                        where MSG.Code == "DISCLAIMER_TEXT"
                                        where MSG.CompanyId == 0
                                        select MSG)
                                  .Union(from MSG in _context.Set<EmailTemplate>()
                                         where MSG.Code == "DISCLAIMER_TEXT" && MSG.CompanyId == companyId
                                         select MSG)
                                         .OrderByDescending(o => o.CompanyId).FirstOrDefault();

                            if (desc != null)
                            {
                                htmlContent.Append(desc.HtmlData.ToString());
                            }
                        }
                        htmlContent.AppendLine("</body></html>");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return htmlContent;
        }

        public string LookupWithKey(string key, string Default = "")
        {
            try
            {
                Dictionary<string, string> Globals = CCConstants.GlobalVars;
                if (Globals.ContainsKey(key))
                {
                    return Globals[key];
                }

                var LKP = (from L in _context.Set<SysParameter>()
                           where L.Name == key
                           select L).FirstOrDefault();
                if (LKP != null)
                {
                    Default = LKP.Value;
                }
                return Default;
            }
            catch (Exception ex)
            {
                throw new SysParameterNotFound(companyId, userId);
            }
        }

        public string UserName(UserFullName strUserName)
        {
            if (strUserName != null)
            {
                return strUserName.Firstname + " " + strUserName.Lastname;
            }
            return "";
        }
        public async Task<int> IncidentNote(int ObjectID, string NoteType, string Notes, int CompanyID, int UserID)
        {
            try
            {
                IncidentTaskNote Note = new IncidentTaskNote()
                {
                    UserId = UserID,
                    ObjectId = ObjectID,
                    CompanyId = CompanyID,
                    IncidentTaskNotesId = ObjectID,
                    NoteType = NoteType,
                    Notes = Notes,
                    CreatedDate = DateTime.Now,
                };
                await _context.AddAsync(Note);
                await _context.SaveChangesAsync();
                return Note.IncidentTaskNotesId;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
        }

        public string GetCompanyParameter(string key, int companyId, string Default = "", string customerId = "")
        {
            try
            {
                key = key.ToUpper();

                if (companyId > 0)
                {
                    var LKP = (from CP in _context.Set<CompanyParameter>()
                               where CP.Name == key && CP.CompanyId == companyId
                               select CP).FirstOrDefault();
                    if (LKP != null)
                    {
                        Default = LKP.Value;
                    }
                    else
                    {
                        var LPR = (from CP in _context.Set<LibCompanyParameters>()
                                   where CP.Name == key
                                   select CP).FirstOrDefault();
                        if (LPR != null)
                        {
                            Default = LPR.Value;
                        }
                        else
                        {
                            Default = LookupWithKey(key, Default);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(key))
                {
                    var cmp = _context.Set<Company>().Where(w => w.CustomerId == customerId).FirstOrDefault();
                    if (cmp != null)
                    {
                        var LKP = (from CP in _context.Set<CompanyParameter>()
                                   where CP.Name == key && CP.CompanyId == cmp.CompanyId
                                   select CP).FirstOrDefault();
                        if (LKP != null)
                        {
                            Default = LKP.Value;
                        }
                    }
                    else
                    {
                        Default = "NOT_EXIST";
                    }
                }

                return Default;
            }
            catch (Exception ex)
            {
                throw new CompanyParameterNotFoundException(companyId, userId);
            }
        }

        public string[] CCRoles(bool addKeyHolder = false, bool addUser = false)
        {
            List<string> rolelist = new List<string> { "ADMIN", "SUPERADMIN" };
            if (addKeyHolder)
                rolelist.Add("KEYHOLDER");

            if (addUser)
                rolelist.Add("USER");

            return rolelist.ToArray();
        }

        public string getapiversion()
        {
            string tapiversion = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["WebApiVersion"])!;
            return (string.IsNullOrEmpty(tapiversion) ? "" : tapiversion + "/");
        }

        public string PWDencrypt(string strPwdString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(strPwdString));

            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public string ToCurrency(decimal Amount, int points = 2)
        {
            return "&pound;" + Amount.ToString("n" + points);
        }

        public DateTimeOffset GetDateTimeOffset(DateTime crTime, string timeZoneId = "GMT Standard Time")
        {
            if (crTime.Year <= 2000)
                return crTime;

            if (crTime.Year > 3000)
            {
                crTime = DateTime.MaxValue.AddHours(-48);
            }

            TimeZoneInfo cet = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var offset = cet.GetUtcOffset(crTime);

            DateTimeOffset newvals = new DateTimeOffset(new DateTime(crTime.Year, crTime.Month, crTime.Day, crTime.Hour, crTime.Minute, crTime.Second, crTime.Millisecond));

            DateTimeOffset convertedtime = newvals.ToOffset(offset);

            return convertedtime;
        }

        public DateTime GetLocalTime(string timeZoneId, DateTime? paramTime = null)
        {
            if (string.IsNullOrEmpty(timeZoneId))
                timeZoneId = "GMT Standard Time";

            DateTime retDate = DateTime.Now.ToUniversalTime();

            DateTime dateTimeToConvert = new DateTime(retDate.Ticks, DateTimeKind.Unspecified);

            DateTime timeUtc = DateTime.UtcNow;

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            retDate = TimeZoneInfo.ConvertTimeFromUtc(dateTimeToConvert, cstZone);

            return retDate;
        }

        public void CreateObjectRelationship(int targetObjectId, int sourceObjectId, string relationName, int companyId, int createdUpdatedBy, string timeZoneId, string relatinFilter = "")
        {
            try
            {
                if (relationName.ToUpper() == "GROUP" || relationName.ToUpper() == "LOCATION")
                {
                    if (targetObjectId > 0)
                    {
                        int newSourceObjectId = 0;

                        var ObjMapId = (from OM in _context.Set<ObjectMapping>()
                                        join OBJ in _context.Set<Core.Models.Object>() on OM.SourceObjectId equals OBJ.ObjectId
                                        where OBJ.ObjectTableName == relationName
                                        select OM).Select(a => a.ObjectMappingId).FirstOrDefault();

                        if (sourceObjectId > 0)
                        {
                            newSourceObjectId = sourceObjectId;
                            CreateNewObjectRelation(newSourceObjectId, targetObjectId, ObjMapId, createdUpdatedBy, timeZoneId, companyId);
                        }

                        if (!string.IsNullOrEmpty(relatinFilter))
                        {
                            if (relationName.ToUpper() == "GROUP")
                            {
                                newSourceObjectId = _context.Set<Core.Groups.Group>().Where(t => t.GroupName == relatinFilter && t.CompanyId == companyId).Select(t => t.GroupId).FirstOrDefault();
                            }
                            else if (relationName.ToUpper() == "LOCATION")
                            {
                                newSourceObjectId = _context.Set<Location>().Where(t => t.LocationName == relatinFilter && t.CompanyId == companyId).Select(t => t.LocationId).FirstOrDefault();
                            }
                            CreateNewObjectRelation(newSourceObjectId, targetObjectId, ObjMapId, createdUpdatedBy, timeZoneId, companyId);
                        }
                    }
                }
                else if (relationName.ToUpper() == "DEPARTMENT")
                {
                    UpdateUserDepartment(targetObjectId, sourceObjectId, createdUpdatedBy, companyId, timeZoneId);
                }
            }
            catch (Exception ex)
            {
                throw new RelationNameNotFoundException(companyId, userId);
            }
        }

        private void UpdateUserDepartment(int userId, int departmentId, int createdUpdatedBy, int companyId, string timeZoneId)
        {
            try
            {
                var user = _context.Set<User>().Where(t => t.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    user.DepartmentId = departmentId;
                    user.UpdatedBy = createdUpdatedBy;
                    user.UpdatedOn = GetDateTimeOffset(DateTime.Now, timeZoneId);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserNotFoundException(companyId, userId);
            }
        }
        public void CreateNewObjectRelation(int sourceObjectId, int targetObjectId, int objMapId, int createdUpdatedBy, string timeZoneId, int companyId)
        {
            try
            {
                bool isAllObjeRelationExist = _context.Set<ObjectRelation>().Where(t => t.TargetObjectPrimaryId == targetObjectId
                && t.ObjectMappingId != objMapId
                && t.SourceObjectPrimaryId == sourceObjectId).Any();

                if (!isAllObjeRelationExist)
                {
                    ObjectRelation tblDepObjRel = new ObjectRelation()
                    {
                        TargetObjectPrimaryId = targetObjectId,
                        ObjectMappingId = objMapId,
                        SourceObjectPrimaryId = sourceObjectId,
                        CreatedBy = createdUpdatedBy,
                        UpdatedBy = createdUpdatedBy,
                        CreatedOn = System.DateTime.Now,
                        UpdatedOn = GetLocalTime(timeZoneId, System.DateTime.Now),
                        ReceiveOnly = false
                    };
                    _context.Set<ObjectRelation>().Add(tblDepObjRel);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new DuplicateEntryException("Dublicate Object Relation");
            }
        }

        public int AddPwdChangeHistory(int userId, string newPassword)
        {
            try
            {
                PasswordChangeHistory PH = new PasswordChangeHistory();
                PH.UserId = userId;
                PH.LastPassword = newPassword;
                PH.ChangedDateTime = GetDateTimeOffset(DateTime.Now);
                _context.Set<PasswordChangeHistory>().Add(PH);
                _context.SaveChanges();
                return PH.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTimeOffset ConvertToLocalTime(string timezoneId, DateTimeOffset paramTime)
        {
            try
            {
                DateTimeOffset retDate = paramTime.ToUniversalTime();

                DateTime dateTimeToConvert = new DateTime(retDate.Ticks, DateTimeKind.Unspecified);

                DateTime timeUtc = DateTime.UtcNow;

                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                retDate = TimeZoneInfo.ConvertTimeFromUtc(dateTimeToConvert, cstZone);

                return retDate;
            }
            catch (Exception ex) { throw ex; }
            return DateTime.Now;
        }
        //Todo: implement this based on the scheduler
        public void DeleteScheduledJob(string jobName, string group)
        {
            //try
            //{
            //    ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
            //    IScheduler _scheduler = schedulerFactory.GetScheduler().Result;
            //    _scheduler.DeleteJob(new JobKey(JobName, Group));
            //}
            //catch (Exception ex)
            //{
            //    catchException(ex);
            //}
        }

        public string GetTimeZoneVal(int UserId)
        {
            try
            {
                string tmpZoneVal = "GMT Standard Time";
                var userInfo = (from U in _context.Set<User>()
                                join C in _context.Set<Company>() on U.CompanyId equals C.CompanyId
                                join T in _context.Set<StdTimeZone>() on C.TimeZone equals T.TimeZoneId
                                where U.UserId == UserId
                                select new
                                {
                                    UserTimezone = T.ZoneLabel
                                }).FirstOrDefault();
                if (userInfo != null)
                {
                    tmpZoneVal = userInfo.UserTimezone;
                }

                return tmpZoneVal;
            }
            catch (Exception ex)
            {
                throw new UserNotFoundException(companyId, userId);
            }
            return "GMT Standard Time";
        }

        public string Left(string str, int lngth, int stpoint = 0)
        {
            return str.Substring(stpoint, Math.Min(str.Length, lngth));
        }

        public string FixMobileZero(string strNumber)
        {
            strNumber = (strNumber == null) ? string.Empty : Regex.Replace(strNumber, @"\D", string.Empty);
            strNumber = Left(strNumber, 1) == "0" ? Left(strNumber, strNumber.Length - 1, 1) : strNumber;
            return strNumber;
        }

        public bool verifyLength(string str, int minLength, int maxLength)
        {
            if (str.Length >= minLength && str.Length <= maxLength)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPropertyExist(dynamic settings, string name)
        {
            return settings.GetType().GetProperty(name) != null;
        }

        public LatLng GetCoordinates(string address)
        {
            LatLng lt = new LatLng();
            lt.Lat = "0";
            lt.Lng = "0";
            if (!string.IsNullOrEmpty(address))
            {
                if (address.Length > 5)
                {
                    string apiUrl = LookupWithKey("GOOGLE_MAPS_API_URL");

                    var requestUri = string.Format(apiUrl, Uri.EscapeDataString(address));
                    try
                    {
                        var request = WebRequest.Create(requestUri);
                        var response = request.GetResponse();
                        var xdoc = XDocument.Load(response.GetResponseStream());
                        string sts = xdoc.Element("GeocodeResponse").Element("status").Value.ToString();
                        if (sts == "OK")
                        {
                            var result = xdoc.Element("GeocodeResponse").Element("result");
                            var locationElement = result.Element("geometry").Element("location");
                            lt.Lat = locationElement.Element("lat").Value.ToString();
                            lt.Lng = locationElement.Element("lng").Value.ToString();
                        }

                        lt.Lat = Left(lt.Lat, 15);
                        lt.Lng = Left(lt.Lng, 15);
                        return lt;
                    }
                    catch (Exception ex)
                    {
                        return lt;
                    }
                }
                else
                {
                    return lt;
                }
            }
            else
            {
                return lt;
            }
        }

        public string GetTimeZoneByCompany(int companyId)
        {
            try
            {
                string tmpZoneVal = "GMT Standard Time";
                var Companytime = (from C in _context.Set<Company>()
                                   join T in _context.Set<StdTimeZone>() on C.TimeZone equals T.TimeZoneId
                                   where C.CompanyId == companyId
                                   select new
                                   {
                                       CompanyTimezone = T.ZoneLabel
                                   }).FirstOrDefault();
                if (Companytime != null)
                {
                    tmpZoneVal = Companytime.CompanyTimezone;
                }
                return tmpZoneVal;
            }
            catch (Exception ex) { throw ex; }
            return "GMT Standard Time";
        }

        public bool connectUNCPath(string UNCPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(UNCPath))
                    UNCPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UNCPath"]);
                if (!string.IsNullOrEmpty(UseUNC))
                    UseUNC = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UseUNC"]);
                if (!string.IsNullOrEmpty(strUncUsername))
                    strUncUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncUserName"]);
                if (!string.IsNullOrEmpty(strUncPassword))
                    strUncPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncPassword"]);

                if (UseUNC == "true")
                {
                    UNCAccessWithCredentials.disconnectRemote(@UNCPath);
                    if (string.IsNullOrEmpty(UNCAccessWithCredentials.connectToRemote(@UNCPath, strUncUsername, strUncPassword)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public string PureAscii(string str, bool KeepAccent = false)
        {
            if (!KeepAccent)
            {
                return Regex.Replace(str, @"[^\u001F-\u007F]", string.Empty);
            }
            else
            {
                return Regex.Replace(str, @"[^[a-zA-Z\u00C0-\u017F]+,\s[a-zA-Z\u00C0-\u017F\p{L}]+$", string.Empty);
            }
        }

        public string RandomPassword(int length = 8, int complexity = 4)
        {
            RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
            // Define the possible character classes where complexity defines the number
            // of classes to include in the final output.
            char[][] classes =
                                {
                                @"abcdefghijklmnopqrstuvwxyz".ToCharArray(),
                                @"ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                                @"0123456789".ToCharArray(),
                                @"!#$%&*@^".ToCharArray(),
                                };

            complexity = Math.Max(1, Math.Min(classes.Length, complexity));
            if (length < complexity)
                throw new ArgumentOutOfRangeException("length");

            char[] allchars = classes.Take(complexity).SelectMany(c => c).ToArray();
            byte[] bytes = new byte[allchars.Length];
            csp.GetBytes(bytes);
            for (int i = 0; i < allchars.Length; i++)
            {
                char tmp = allchars[i];
                allchars[i] = allchars[bytes[i] % allchars.Length];
                allchars[bytes[i] % allchars.Length] = tmp;
            }

            // Create the random values to select the characters
            Array.Resize(ref bytes, length);
            char[] result = new char[length];

            while (true)
            {
                csp.GetBytes(bytes);
                // Obtain the character of the class for each random byte
                for (int i = 0; i < length; i++)
                    result[i] = allchars[bytes[i] % allchars.Length];

                // Verify that it does not start or end with whitespace
                if (Char.IsWhiteSpace(result[0]) || Char.IsWhiteSpace(result[(length - 1) % length]))
                    continue;

                string testResult = new string(result);
                // Verify that all character classes are represented
                if (0 != classes.Take(complexity).Count(c => testResult.IndexOfAny(c) < 0))
                    continue;

                return testResult;
            }
        }
        public void RemoveUserObjectRelation(string RelationName, int UserId, int SourceObjectId, int CompanyId, int CurrentUserId, string TimeZoneId)
        {
            try
            {
                if (RelationName.ToUpper() == "GROUP" || RelationName.ToUpper() == "LOCATION")
                {
                    var ObjMapId = (from OM in _context.Set<ObjectMapping>()
                                    join OBJ in _context.Set<Core.Models.Object>() on OM.SourceObjectId equals OBJ.ObjectId
                                    where OBJ.ObjectTableName == RelationName
                                    select OM).Select(a => a.ObjectMappingId).FirstOrDefault();

                    var getRelationRec = (from OR in _context.Set<ObjectRelation>()
                                          where OR.ObjectMappingId == ObjMapId && OR.TargetObjectPrimaryId == UserId &&
                                          OR.SourceObjectPrimaryId == SourceObjectId
                                          select OR).FirstOrDefault();
                    if (getRelationRec != null)
                    {
                        _context.Set<ObjectRelation>().Remove(getRelationRec);
                        _context.SaveChanges();
                    }
                }
                else if (RelationName.ToUpper() == "DEPARTMENT")
                {
                    UpdateUserDepartment(UserId, 0, CurrentUserId, CompanyId, TimeZoneId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
       
       
        public void RemoveUserDevice(int UserID, bool TokenReset = false)
        {
            try
            {
                var devices = (from UD in _context.Set<UserDevice>() where UD.UserId == UserID select UD).ToList();
                if (!TokenReset)
                {
                    _context.Set<UserDevice>().RemoveRange(devices);
                }
                else
                {
                    devices.ForEach(s => s.DeviceToken = "");
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async void MessageProcessLog(int MessageID, string EventName, string MethodName = "", string QueueName = "", string AdditionalInfo = "")
        {
            try
            {
                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var pEventName = new SqlParameter("@EventName", EventName);
                var pMethodName = new SqlParameter("@MethodName", MethodName);
                var pQueueName = new SqlParameter("@QueueName", QueueName);
                var pAdditionalInfo = new SqlParameter("@AdditionalInfo", AdditionalInfo);

                await _context.Set<Result>().FromSqlRaw("exec Pro_Message_Process_Log_Insert @MessageID, @EventName, @MethodName, @QueueName, @AdditionalInfo",
                     pMessageID, pEventName, pMethodName, pQueueName, pAdditionalInfo).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CreateLog(string Level, string Message, Exception Ex = null, string Controller = "", string Method = "", int CompanyId = 0)
        {

            if (Level.ToUpper() == "INFO")
            {
                string CreateLog = LookupWithKey("COLLECT_PERFORMANCE_LOG");
                if (CreateLog == "false")
                    return;
            }

            LogicalThreadContext.Properties["ControllerName"] = Controller;
            LogicalThreadContext.Properties["MethodName"] = Method;
            LogicalThreadContext.Properties["CompanyId"] = CompanyId;
            if (Level.ToUpper() == "ERROR")
            {
                Logger.Error(Message, Ex);
            }
            else if (Level.ToUpper() == "DEBUG")
            {
                Logger.Debug(Message, Ex);
            }
            else if (Level.ToUpper() == "INFO")
            {
                Logger.Info(Message, Ex);
            }



            if (Ex != null)
                Console.WriteLine(Message + Ex.ToString());
        }

        public void UpdateLog(string strErrorID, string strErrorMessage, string strControllerName, string strMethodName, int intCompanyId)
        {
            try
            {
                CreateLog("INFO", Left(strErrorMessage, 8000), null, strControllerName, strMethodName, intCompanyId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Getconfig(string key, string DefaultVal = "")
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.ConnectionStrings.ToString();
                if (value != null)
                {
                    return value;
                }
                else
                {
                    return DefaultVal;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FormatMobile(string ISD, string Mobile)
        {
            if (!string.IsNullOrEmpty(Mobile))
            {
                Mobile = Mobile.TrimStart('0').TrimStart('+');
                if (Mobile.Length > 4)
                {
                    ISD = ISD.TrimStart('+').TrimStart('0');
                    return "+" + ISD + Mobile;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public bool IsTrue(string BoolVal, bool Default = true)
        {
            if (BoolVal == "true")
            {
                return true;
            }
            else if (BoolVal == "false")
            {
                return false;
            }
            else
            {
                return Default;
            }
        }
        public int ChunkString(string str, int chunkSize)
        {
            int result = 1;
            Math.DivRem(str.Length, chunkSize, out result);
            int z = (str.Length / chunkSize) + (result > 0 ? 1 : 0);
            return z;
        }
        public dynamic InitComms(string API_CLASS, string APIClass = "", string ClientId = "", string ClientSecret = "")
        {
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
            try
            {

                int RetryCount = 2;
                int.TryParse(DBC.LookupWithKey(API_CLASS + "_MESSAGE_RETRY_COUNT"), out RetryCount);

                if (string.IsNullOrEmpty(APIClass))
                    APIClass = DBC.LookupWithKey(API_CLASS + "_API_CLASS");

                if (string.IsNullOrEmpty(ClientId))
                    ClientId = DBC.LookupWithKey(API_CLASS + "_CLIENTID");

                if (string.IsNullOrEmpty(ClientSecret))
                    ClientSecret = DBC.LookupWithKey(API_CLASS + "_CLIENT_SECRET");

                string[] TmpClass = APIClass.Trim().Split('|');

                string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");

                Assembly assembly = Assembly.LoadFrom(binPath + "\\" + TmpClass[0]);
                Type type = assembly.GetType(TmpClass[1]);
                dynamic CommsAPI = Activator.CreateInstance(type);

                CommsAPI.ClientId = ClientId;
                CommsAPI.Secret = ClientSecret;
                CommsAPI.RetryCount = RetryCount;

                return CommsAPI;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
        public async Task<List<NotificationUserList>> GetUniqueUsers(List<NotificationUserList> List1, List<NotificationUserList> List2, bool ParticipantCheck = true)
        {
            try
            {
                if (List2 != null)
                {
                    if (List2.Count > 0)
                    {
                        List1.RemoveAll(x => List2.Exists(y => y.UserId == x.UserId && y.IsTaskRecipient == ParticipantCheck));
                        List1 = List1.Union(List2).ToList();
                    }
                }
                List1 = List1.GroupBy(g => new { g.UserId, g.IsTaskRecipient })
                    .Select(g => g.First())
                    .ToList();
                List1.RemoveAll(x => List1.Exists(y => y.UserId == x.UserId && y.IsTaskRecipient == true && x.IsTaskRecipient == false));
                return List1;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<AckOption>> GetAckOptions(int MessageID)
        {
            try
            {
                return await _context.Set<ActiveMessageResponse>()
                        .Where(MR => MR.MessageId == MessageID)
                        .Select(MR => new AckOption()
                        {
                            ResponseId = MR.ResponseId,
                            ResponseLabel = MR.ResponseLabel
                        }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
        public string GetValueByIndex(List<string> ValueList, int IndexVal)
        {
            try
            {

                bool isModed = false;
                if (IndexVal > ValueList.Count)
                {
                    IndexVal %= ValueList.Count;
                    isModed = true;
                }

                if (IndexVal == 0 && isModed == true)
                    IndexVal = ValueList.Count;

                if (IndexVal < 0)
                    IndexVal = 0;

                if (ValueList.ElementAtOrDefault(IndexVal) != null)
                {
                    return ValueList.ElementAtOrDefault(IndexVal);
                }
                else
                {
                    return GetValueByIndex(ValueList, 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return "+441212855004";
            }
        }
        public async Task LocalException(string Error, string Message, string Controller = "", string Method = "", int CompanyId = 0)
        {
            ExceptionLog el = new ExceptionLog();
            el.ControllerName = Controller;
            el.MethodName = Method;
            el.CompanyId = CompanyId;
            el.CreatedBy = 0;
            el.EntryDate = DateTimeOffset.Now;
            el.ErrorId = Error;
            el.ErrorMessage = Message;
            await _context.AddAsync(el);
            await _context.SaveChangesAsync();
        }
        public bool IsDayLightOn(DateTime thisDate)
        {
            try
            {
                TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                bool isDaylight = tst.IsDaylightSavingTime(thisDate);
                DateTime f2 = TimeZoneInfo.ConvertTime(thisDate, tst);
                var isSummer = tst.IsDaylightSavingTime(f2);
                return isDaylight;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        
        public void CancelJobsByGroup(string JobGroup)
        {
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(JobGroup);
                var jobKeys = _scheduler.GetJobKeys(groupMatcher).Result;
                foreach (var jobKey in jobKeys)
                {

                    _scheduler.DeleteJob(jobKey);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteOldFiles(string dirName)
        {
            string[] files = Directory.GetFiles(@dirName);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddDays(-1))
                    fi.Delete();
            }
        }
        public DateTimeOffset GetNextReviewDate(DateTimeOffset CurrentReviewDate, int CompanyID, int ReminderCount, out int ReminderCounter)
        {
            try
            {
                ReminderCounter = 0;
                int reminder1 = 30;
                int reminder2 = 15;
                int reminder3 = 7;

                int.TryParse(GetCompanyParameter("SOP_DOCUMENT_REMINDER_1", CompanyID), out reminder1);
                int.TryParse(GetCompanyParameter("SOP_DOCUMENT_REMINDER_2", CompanyID), out reminder2);
                int.TryParse(GetCompanyParameter("SOP_DOCUMENT_REMINDER_3", CompanyID), out reminder3);

                DateTime CheckDate = CurrentReviewDate.AddDays(-reminder1).Date;

                if (CheckDate.Date >= DateTime.Now.Date && ReminderCount == 0)
                {
                    ReminderCounter = 1;
                    return CheckDate;
                }

                if (CurrentReviewDate.AddDays(-reminder2).Date >= DateTime.Now.Date && (ReminderCount == 0 || ReminderCount == 1))
                {
                    ReminderCounter = 2;
                    return CurrentReviewDate.AddDays(-reminder2).Date;
                }

                if (CurrentReviewDate.AddDays(-reminder3).Date >= DateTime.Now.Date && (ReminderCount == 0 || ReminderCount == 1 || ReminderCount == 2))
                {
                    ReminderCounter = 3;
                    return CurrentReviewDate.AddDays(-reminder3).Date;
                }

                return CurrentReviewDate.AddYears(-1).Date;

            }
            catch (Exception ex)
            {
                ReminderCounter = 0;
                return CurrentReviewDate.AddYears(-1).Date;
            }
        }
        public DateTimeOffset GetNextReviewDate(DateTimeOffset CurrentDateTime, string Frequency)
        {
            DateTimeOffset NewReviewDate = CurrentDateTime;

            if (string.IsNullOrEmpty(Frequency))
                Frequency = "MONTH";

            switch (Frequency)
            {
                case "WEEK":
                    NewReviewDate = CurrentDateTime.AddDays(7);
                    break;
                case "MONTH":
                    NewReviewDate = CurrentDateTime.AddMonths(1);
                    break;
                case "QUARTER":
                    NewReviewDate = CurrentDateTime.AddMonths(3);
                    break;
                case "YEAR":
                    NewReviewDate = CurrentDateTime.AddYears(1);
                    break;
            }
            return NewReviewDate;
        }
        public async Task CreateSOPReviewReminder(int IncidentID, int SOPHeaderID, int CompanyID, DateTimeOffset NextReviewDate, string ReviewFrequency, int ReminderCount)
        {
            try
            {

                DeleteScheduledJob("SOP_REVIEW_" + SOPHeaderID, "REVIEW_REMINDER");

                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "SOP_REVIEW_" + SOPHeaderID;
                string taskTrigger = "SOP_REVIEW_" + SOPHeaderID;

                var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "REVIEW_REMINDER", typeof(SOPReviewJob));
                jobDetail.JobDataMap["IncidentID"] = IncidentID;
                jobDetail.JobDataMap["SOPHeaderID"] = SOPHeaderID;

                int Counter = 0;
                DateTimeOffset DateCheck =GetNextReviewDate(NextReviewDate, CompanyID, ReminderCount, out Counter);
                jobDetail.JobDataMap["Counter"] = Counter;

                var sop_head =  _context.Set<Sopheader>().Where(SH=> SH.SopheaderId == SOPHeaderID).FirstOrDefault();
                sop_head.ReminderCount = Counter;
                _context.Update(sop_head);
                await _context.SaveChangesAsync();

                //if(DateCheck.Date >= DateTime.Now.Date && Counter <= 3) {
                if (DateTimeOffset.Compare(DateCheck, GetDateTimeOffset(DateTime.Now)) >= 0 && Counter <= 3)
                {
                    //string TimeZoneVal = DBC.GetTimeZoneByCompany(CompanyID);
                    //DateCheck = DBC.GetServerTime(TimeZoneVal, DateCheck);

                    if (DateCheck < DateTime.Now)
                        DateCheck = DateTime.Now.AddMinutes(5);

                    //DBC.UpdateLog("0", jobName + ", Starting at: " + DateCheck.ToUniversalTime().ToString(), "CreateTasksReviewReminder", "CreateTasksReviewReminder", 0);

                    ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                                                              .WithIdentity(taskTrigger, "REVIEW_REMINDER")
                                                              .StartAt(DateCheck.ToUniversalTime())
                                                              .ForJob(jobDetail)
                                                              .Build();
                    _scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                {
                    DateTimeOffset NewReviewDate = GetNextReviewDate(NextReviewDate, ReviewFrequency);

                    if (sop_head != null)
                    {
                        sop_head.ReviewDate = NewReviewDate;
                        sop_head.ReminderCount = 0;
                        await _context.SaveChangesAsync();
                        await CreateSOPReviewReminder(IncidentID, SOPHeaderID, CompanyID, NewReviewDate, ReviewFrequency, 0);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
