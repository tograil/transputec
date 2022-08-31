using CrisesControl.Core.Companies;
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
using CrisesControl.Core.Import;

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
                    var content =  _context.Set<EmailTemplate>()
                                   .Where(MSG=> MSG.Code == fileCode && MSG.CompanyId == 0
                                  )
                                   .Union( _context.Set<EmailTemplate>()
                                          .Where(MSG=> MSG.Code == fileCode && MSG.CompanyId == companyId
                                          ))
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
                            var desc =  _context.Set<EmailTemplate>()
                                        .Where(MSG=> MSG.Code == "DISCLAIMER_TEXT" && MSG.CompanyId == 0
                                        )
                                  .Union( _context.Set<EmailTemplate>()
                                         .Where(MSG=> MSG.Code == "DISCLAIMER_TEXT" && MSG.CompanyId == companyId
                                         ))
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
        public async Task<int> IncidentNote(int objectId, string noteType, string notes, int companyId, int userId)
        {
            try
            {
                IncidentTaskNote Note = new IncidentTaskNote()
                {
                    UserId = userId,
                    ObjectId = objectId,
                    CompanyId = companyId,
                    IncidentTaskNotesId = objectId,
                    NoteType = noteType,
                    Notes = notes,
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
                    var LKP =  _context.Set<CompanyParameter>()
                               .Where(CP=> CP.Name == key && CP.CompanyId == companyId
                               ).FirstOrDefault();
                    if (LKP != null)
                    {
                        Default = LKP.Value;
                    }
                    else
                    {
                        var LPR =  _context.Set<LibCompanyParameters>()
                                   .Where(CP=> CP.Name == key
                                   ).FirstOrDefault();
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
                        var LKP =  _context.Set<CompanyParameter>()
                                   .Where(CP=> CP.Name == key && CP.CompanyId == cmp.CompanyId
                                   ).FirstOrDefault();
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

                        var ObjMapId =  _context.Set<ObjectMapping>()
                                       .Include(OBJ=>OBJ.Object) 
                                       .Where(OBJ=> OBJ.Object.ObjectTableName == relationName)
                                       .Select(a => a.ObjectMappingId).FirstOrDefault();

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

        public string GetTimeZoneVal(int userId)
        {
            try
            {
                string tmpZoneVal = "GMT Standard Time";
                var userInfo = _context.Set<User>().Include(C=>C.Company)                               
                                .Where(U=> U.UserId == userId)
                                .Select(T=> new
                                {
                                    UserTimezone = T.Company.StdTimeZone.ZoneLabel
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
        public string GetTimeZoneByCompany(int companyId)
        {
            try
            {
                string tmpZoneVal = "GMT Standard Time";
                var Companytime =  _context.Set<Company>().Include(st=>st.StdTimeZone)
                                   .Where(C=> C.CompanyId == companyId)
                                   .Select(T=> new
                                   {
                                       CompanyTimezone = T.StdTimeZone.ZoneLabel
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
        public void CreateLog(string level, string message, Exception ex = null, string controller = "", string method = "", int companyId = 0)
        {

            if (level.ToUpper() == "INFO")
            {
                string CreateLog = LookupWithKey("COLLECT_PERFORMANCE_LOG");
                if (CreateLog == "false")
                    return;
            }

            LogicalThreadContext.Properties["ControllerName"] = controller;
            LogicalThreadContext.Properties["MethodName"] = method;
            LogicalThreadContext.Properties["CompanyId"] = companyId;
            if (level.ToUpper() == "ERROR")
            {
                Logger.Error(message, ex);
            }
            else if (level.ToUpper() == "DEBUG")
            {
                Logger.Debug(message, ex);
            }
            else if (level.ToUpper() == "INFO")
            {
                Logger.Info(message, ex);
            }



            if (ex != null)
                Console.WriteLine(message + ex.ToString());
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
        public async Task GetSetCompanyComms(int companyId)
        {
            try
            {
                DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
                var comp_pp = await  _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == companyId).FirstOrDefaultAsync();
                var comp = await _context.Set<Company>().Where(C=> C.CompanyId == companyId).FirstOrDefaultAsync();
                if (comp_pp != null && comp != null)
                {

                    if (comp.Status == 1)
                    {
                        bool sendAlert = false;

                        DateTimeOffset LastUpdate = comp_pp.UpdatedOn;

                        List<string> stopped_comms = new List<string>();

                        if (comp_pp.MinimumEmailRate > 0)
                        {
                            stopped_comms.Add("EMAIL");
                        }
                        if (comp_pp.MinimumPhoneRate > 0)
                        {
                            stopped_comms.Add("PHONE");
                        }
                        if (comp_pp.MinimumTextRate > 0)
                        {
                            stopped_comms.Add("TEXT");
                        }
                        if (comp_pp.MinimumPushRate > 0)
                        {
                            stopped_comms.Add("PUSH");
                        }

                        if (comp_pp.CreditBalance > comp_pp.MinimumBalance)
                        { //Have positive balance + More than the minimum balance required.
                            comp.CompanyProfile = "SUBSCRIBED";
                            _set_comms_status(companyId, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < -comp_pp.CreditLimit)
                        { //Used the overdraft amount as well, so stop their SMS and Phone
                            comp.CompanyProfile = "STOP_MESSAGING";
                            sendAlert = true;
                            _set_comms_status(companyId, stopped_comms, false);
                        }
                        else if (comp_pp.CreditBalance < 0 && comp_pp.CreditBalance > -comp_pp.CreditLimit)
                        { //Using the overdraft facility, can still use the system
                            comp.CompanyProfile = "ON_CREDIT";
                            sendAlert = true;
                            _set_comms_status(companyId, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < comp_pp.MinimumBalance)
                        { //Less than the minimum balance, just send an alert, can still use the system.
                            comp.CompanyProfile = "LOW_BALANCE";
                            sendAlert = true;
                            _set_comms_status(companyId, stopped_comms, true);
                        }
                        comp_pp.UpdatedOn = GetDateTimeOffset(DateTime.Now);
                        _context.Update(comp_pp);
                       await  _context.SaveChangesAsync();

                        if (DateTimeOffset.Now.Subtract(LastUpdate).TotalHours < 24)
                        {
                            sendAlert = false;
                        }

                        string CommsDebug = LookupWithKey("COMMS_DEBUG_MODE");

                        if (sendAlert && CommsDebug == "false")
                        {
                            SendEmail SDE = new SendEmail(_context,DBC);
                            await SDE.UsageAlert(companyId);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void _set_comms_status(int companyId, List<string> methods, bool status)
        {
            try
            {
                 _context.Set<CompanyComm>().Include( CO => CO.CommsMethod)
                 .Where(CM=> CM.CompanyId == companyId && methods.Contains(CM.CommsMethod.MethodCode)
                 ).ToList().ForEach(x => x.ServiceStatus = status);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> GetPackageItem(string itemCode, int companyId)
        {
            string retVal = string.Empty;
            itemCode = itemCode.Trim();
            var ItemRec = await  _context.Set<CompanyPackageItem>().Where(PI=> PI.ItemCode == itemCode && PI.CompanyId == companyId).FirstOrDefaultAsync();
            if (ItemRec != null)
            {
                retVal = ItemRec.ItemValue;
            }
            else
            {
                var LibItemRec = await _context.Set<LibPackageItem>().Where(PI=> PI.ItemCode == itemCode).FirstOrDefaultAsync();
                retVal = LibItemRec.ItemValue;
            }
            return retVal;
        }
        public DateTimeOffset GetNextReviewDate(DateTimeOffset currentDateTime, string frequency)
        {
            DateTimeOffset NewReviewDate = currentDateTime;

            if (string.IsNullOrEmpty(frequency))
                frequency = "MONTH";

            switch (frequency)
            {
                case "WEEK":
                    NewReviewDate = currentDateTime.AddDays(7);
                    break;
                case "MONTH":
                    NewReviewDate = currentDateTime.AddMonths(1);
                    break;
                case "QUARTER":
                    NewReviewDate = currentDateTime.AddMonths(3);
                    break;
                case "YEAR":
                    NewReviewDate = currentDateTime.AddYears(1);
                    break;
            }
            return NewReviewDate;
        }
        public DateTimeOffset GetNextReviewDate(DateTimeOffset currentReviewDate, int companyID, int reminderCount, out int reminderCounter)
        {
            try
            {
                reminderCounter = 0;
                int reminder1 = 30;
                int reminder2 = 15;
                int reminder3 = 7;

                int.TryParse(GetCompanyParameter("SOP_DOCUMENT_REMINDER_1", companyID), out reminder1);
                int.TryParse(GetCompanyParameter("SOP_DOCUMENT_REMINDER_2", companyID), out reminder2);
                int.TryParse(GetCompanyParameter("SOP_DOCUMENT_REMINDER_3", companyID), out reminder3);

                DateTime CheckDate = currentReviewDate.AddDays(-reminder1).Date;

                if (CheckDate.Date >= DateTime.Now.Date && reminderCount == 0)
                {
                    reminderCounter = 1;
                    return CheckDate;
                }

                if (currentReviewDate.AddDays(-reminder2).Date >= DateTime.Now.Date && (reminderCount == 0 || reminderCount == 1))
                {
                    reminderCounter = 2;
                    return currentReviewDate.AddDays(-reminder2).Date;
                }

                if (currentReviewDate.AddDays(-reminder3).Date >= DateTime.Now.Date && (reminderCount == 0 || reminderCount == 1 || reminderCount == 2))
                {
                    reminderCounter = 3;
                    return currentReviewDate.AddDays(-reminder3).Date;
                }

                return currentReviewDate.AddYears(-1).Date;

            }
            catch (Exception ex)
            {
                throw ex;
                reminderCounter = 0;
                return currentReviewDate.AddYears(-1).Date;
            }
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

        public bool connectUNCPath(string uncPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(uncPath))
                    uncPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UNCPath"]);
                if (!string.IsNullOrEmpty(UseUNC))
                    UseUNC = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UseUNC"]);
                if (!string.IsNullOrEmpty(strUncUsername))
                    strUncUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncUserName"]);
                if (!string.IsNullOrEmpty(strUncPassword))
                    strUncPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncPassword"]);

                if (UseUNC == "true")
                {
                    UNCAccessWithCredentials.disconnectRemote(@uncPath);
                    if (string.IsNullOrEmpty(UNCAccessWithCredentials.connectToRemote(@uncPath, strUncUsername, strUncPassword)))
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
        public string PureAscii(string str, bool keepAccent = false)
        {
            if (!keepAccent)
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
        public void RemoveUserObjectRelation(string relationName, int userId, int sourceObjectId, int companyId, int currentUserId, string timeZoneId)
        {
            try
            {
                if (relationName.ToUpper() == "GROUP" || relationName.ToUpper() == "LOCATION")
                {
                    var ObjMapId =  _context.Set<ObjectMapping>().Include(o=>o.Object) 
                                    .Where(OBJ=> OBJ.Object.ObjectTableName == relationName
                                    ).Select(a => a.ObjectMappingId).FirstOrDefault();

                    var getRelationRec =  _context.Set<ObjectRelation>()
                                          .Where(OR=> OR.ObjectMappingId == ObjMapId && OR.TargetObjectPrimaryId == userId &&
                                          OR.SourceObjectPrimaryId == sourceObjectId).FirstOrDefault();
                    if (getRelationRec != null)
                    {
                        _context.Set<ObjectRelation>().Remove(getRelationRec);
                        _context.SaveChanges();
                    }
                }
                else if (relationName.ToUpper() == "DEPARTMENT")
                {
                    UpdateUserDepartment(userId, 0, currentUserId, companyId, timeZoneId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void RemoveUserDevice(int userId, bool tokenReset = false)
        {
            try
            {
                var devices = (from UD in _context.Set<UserDevice>() where UD.UserId == userId select UD).ToList();
                if (!tokenReset)
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

        
        public async Task MessageProcessLog(int messageId, string eventName, string methodName = "", string queueName = "", string additionalInfo = "")
        {
            try
            {
                var pMessageID = new SqlParameter("@MessageID", messageId);
                var pEventName = new SqlParameter("@EventName", eventName);
                var pMethodName = new SqlParameter("@MethodName", methodName);
                var pQueueName = new SqlParameter("@QueueName", queueName);
                var pAdditionalInfo = new SqlParameter("@AdditionalInfo", additionalInfo);

                await _context.Set<Result>().FromSqlRaw("exec Pro_Message_Process_Log_Insert @MessageID, @EventName, @MethodName, @QueueName, @AdditionalInfo",
                     pMessageID, pEventName, pMethodName, pQueueName, pAdditionalInfo).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }    
        public string Getconfig(string key, string defaultVal = "")
        {
            try
            {
                string value = _context.Database.GetConnectionString();
                if (value != null)
                {
                    return value;
                }
                else
                {
                    return defaultVal;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string FormatMobile(string ISD, string mobile)
        {
            if (!string.IsNullOrEmpty(mobile))
            {
                mobile = mobile.TrimStart('0').TrimStart('+');
                if (mobile.Length > 4)
                {
                    ISD = ISD.TrimStart('+').TrimStart('0');
                    return "+" + ISD + mobile;
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
        public bool IsTrue(string boolVal, bool Default = true)
        {
            if (boolVal == "true")
            {
                return true;
            }
            else if (boolVal == "false")
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
        public dynamic InitComms(string API_CLASS, string apiClass = "", string clientId = "", string clientSecret = "")
        {
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
            try
            {

                int RetryCount = 2;
                int.TryParse(DBC.LookupWithKey(API_CLASS + "_MESSAGE_RETRY_COUNT"), out RetryCount);

                if (string.IsNullOrEmpty(apiClass))
                    apiClass = DBC.LookupWithKey(API_CLASS + "_API_CLASS");

                if (string.IsNullOrEmpty(clientId))
                    clientId = DBC.LookupWithKey(API_CLASS + "_CLIENTID");

                if (string.IsNullOrEmpty(clientSecret))
                    clientSecret = DBC.LookupWithKey(API_CLASS + "_CLIENT_SECRET");

                string[] TmpClass = apiClass.Trim().Split('|');

                string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");

                Assembly assembly = Assembly.LoadFrom(binPath + "\\" + TmpClass[0]);
                Type type = assembly.GetType(TmpClass[1]);
                dynamic CommsAPI = Activator.CreateInstance(type);

                CommsAPI.ClientId = clientId;
                CommsAPI.Secret = clientSecret;
                CommsAPI.RetryCount = RetryCount;

                return CommsAPI;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
        public async Task<List<NotificationUserList>> GetUniqueUsers(List<NotificationUserList> list1, List<NotificationUserList> list2, bool participantCheck = true)
        {
            try
            {
                if (list2 != null)
                {
                    if (list2.Count > 0)
                    {
                        list1.RemoveAll(x => list2.Exists(y => y.UserId == x.UserId && y.IsTaskRecipient == participantCheck));
                        list1 = list1.Union(list2).ToList();
                    }
                }
                list1 = list1.GroupBy(g => new { g.UserId, g.IsTaskRecipient })
                    .Select(g => g.First())
                    .ToList();
                list1.RemoveAll(x => list1.Exists(y => y.UserId == x.UserId && y.IsTaskRecipient == true && x.IsTaskRecipient == false));
                return list1;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<AckOption>> GetAckOptions(int messageId)
        {
            try
            {
                return await _context.Set<ActiveMessageResponse>()
                        .Where(MR => MR.MessageId == messageId)
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
        public string GetValueByIndex(List<string> valueList, int indexVal)
        {
            try
            {

                bool isModed = false;
                if (indexVal > valueList.Count)
                {
                    indexVal %= valueList.Count;
                    isModed = true;
                }

                if (indexVal == 0 && isModed == true)
                    indexVal = valueList.Count;

                if (indexVal < 0)
                    indexVal = 0;

                if (valueList.ElementAtOrDefault(indexVal) != null)
                {
                    return valueList.ElementAtOrDefault(indexVal);
                }
                else
                {
                    return GetValueByIndex(valueList, 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return "+441212855004";
            }
        }
        public async Task LocalException(string error, string message, string controller = "", string method = "", int companyId = 0)
        {
            ExceptionLog el = new ExceptionLog();
            el.ControllerName = controller;
            el.MethodName = method;
            el.CompanyId = companyId;
            el.CreatedBy = 0;
            el.EntryDate = DateTimeOffset.Now;
            el.ErrorId = error;
            el.ErrorMessage = message;
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

        public void CancelJobsByGroup(string jobGroup)
        {
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(jobGroup);
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
        public async Task CreateSOPReviewReminder(int incidentId, int sopHeaderId, int companyId, DateTimeOffset nextReviewDate, string reviewFrequency, int reminderCount)
        {
            try
            {

                DeleteScheduledJob("SOP_REVIEW_" + sopHeaderId, "REVIEW_REMINDER");

                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "SOP_REVIEW_" + sopHeaderId;
                string taskTrigger = "SOP_REVIEW_" + sopHeaderId;

                var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "REVIEW_REMINDER", typeof(SOPReviewJob));
                jobDetail.JobDataMap["IncidentID"] = incidentId;
                jobDetail.JobDataMap["SOPHeaderID"] = sopHeaderId;

                int Counter = 0;
                DateTimeOffset DateCheck =GetNextReviewDate(nextReviewDate, companyId, reminderCount, out Counter);
                jobDetail.JobDataMap["Counter"] = Counter;

                var sop_head =  _context.Set<Sopheader>().Where(SH=> SH.SopheaderId == sopHeaderId).FirstOrDefault();
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
                  await  _scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                {
                    DateTimeOffset newReviewDate = GetNextReviewDate(nextReviewDate, reviewFrequency);

                    if (sop_head != null)
                    {
                        sop_head.ReviewDate = newReviewDate;
                        sop_head.ReminderCount = 0;
                        await _context.SaveChangesAsync();
                        await CreateSOPReviewReminder(incidentId, sopHeaderId, companyId, newReviewDate, reviewFrequency, 0);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<SocialHandles>> GetSocialServiceProviders()
        {
            try
            {
                List<SocialHandles> SH = new List<SocialHandles>();
                var syspr = await _context.Set<SysParameter>().Where(SP=> SP.Category == "SOCIAL_HANDLE" && SP.Status == 1).ToListAsync();
                foreach (var spvar in syspr)
                {
                    SH.Add(new SocialHandles { ProviderCode = spvar.Name, ProviderName = spvar.Value });
                }
                return SH;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> SegregationWarning(int companyId, int userID, int incidentId)
        {
            var pIncidentId = new SqlParameter("@IncidentID", incidentId);
            var pUserID = new SqlParameter("@UserID", userID);
            var pCompanyId = new SqlParameter("@CompanyID", companyId);

            int SegWarning =await _context.Database.ExecuteSqlRawAsync("SELECT [dbo].[Incident_Segregation](@IncidentID,@UserID,@CompanyID)", pIncidentId, pUserID, pCompanyId);
            return SegWarning;
        }

        public List<SocialIntegraion> GetSocialIntegration(int companyId, string accountType)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pAccountType = new SqlParameter("@AccountType", accountType);

                var result = _context.Set<SocialIntegraion>().FromSqlRaw("EXEC Pro_Get_Social_Integration @CompanyID, @AccountType", pCompanyID, pAccountType).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task SaveParameter(int parameterID, string parameterName, string parameterValue, int currentUserID, int companyID, string timeZoneId)
        {
            try
            {
                if (parameterID > 0)
                {
                    var CompanyParameter = await _context.Set<CompanyParameter>()
                                            .Where(CP=> CP.CompanyId == companyID && CP.CompanyParametersId == parameterID
                                            ).FirstOrDefaultAsync();

                    if (CompanyParameter != null)
                    {
                        //if(!string.IsNullOrEmpty(ParameterName)) {
                        //    CompanyParameter.Name = ParameterName;
                        //}
                        if (parameterValue != null)
                        {
                            if (CompanyParameter.Name == "MAX_PING_KPI" || CompanyParameter.Name == "MAX_INCIDENT_KPI")
                            {
                                CompanyParameter.Value = Convert.ToString(Convert.ToInt32(parameterValue) * 60);
                            }
                            else
                            {
                                CompanyParameter.Value = parameterValue;
                            }
                        }
                        CompanyParameter.UpdatedOn = GetDateTimeOffset(DateTime.Now, timeZoneId);
                        CompanyParameter.UpdatedBy = currentUserID;
                        _context.Update(CompanyParameter);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (!string.IsNullOrEmpty(parameterName))
                {
                    var CompanyParameter = await _context.Set<CompanyParameter>()
                                            .Where(CP=> CP.CompanyId == companyID && CP.Name == parameterName).FirstOrDefaultAsync();

                    if (CompanyParameter != null)
                    {
                        if (parameterValue != null)
                        {
                            if (CompanyParameter.Name == "MAX_PING_KPI" || CompanyParameter.Name == "MAX_INCIDENT_KPI")
                            {
                                CompanyParameter.Value = Convert.ToString(Convert.ToInt32(parameterValue) * 60);
                            }
                            else
                            {
                                CompanyParameter.Value = parameterValue;
                            }
                        }
                        CompanyParameter.UpdatedOn = GetDateTimeOffset(DateTime.Now, timeZoneId);
                        CompanyParameter.UpdatedBy = currentUserID;
                        _context.Update(CompanyParameter);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        int CompanyParametersId = await AddCompanyParameter(parameterName, parameterValue, companyID, currentUserID, timeZoneId);
                    }
                }

                if (parameterName.ToUpper() == "RECHARGE_BALANCE_TRIGGER")
                {
                    var profile = await _context.Set<CompanyPaymentProfile>().Where(CP=> CP.CompanyId == companyID).FirstOrDefaultAsync();
                    if (profile != null)
                    {
                        profile.MinimumBalance = Convert.ToDecimal(parameterValue);
                        _context.Update(profile);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddCompanyParameter(string name, string value, int companyId, int currentUserId, string timeZoneId)
        {
            try
            {
                var comp_param = await _context.Set<CompanyParameter>().Where(CP=> CP.CompanyId == companyId && CP.Name == name).AnyAsync();
                if (!comp_param)
                {
                    CompanyParameter NewCompanyParameters = new CompanyParameter()
                    {
                        CompanyId = companyId,
                        Name = name,
                        Value = value,
                        Status = 1,
                        CreatedBy = currentUserId,
                        UpdatedBy = currentUserId,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = GetDateTimeOffset(DateTime.Now, timeZoneId)
                    };
                    await _context.AddAsync(NewCompanyParameters);
                    await _context.SaveChangesAsync();
                    return NewCompanyParameters.CompanyParametersId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }
        public bool OnTrialStatus(string companyProfile, bool currentTrial)
        {
            if (companyProfile == "SUBSCRIBED")
            {
                return false;
            }
            return companyProfile == "ON_TRIAL" ? true : currentTrial;
        }

        public void GetStartEndDate(bool isThisWeek, bool isThisMonth, bool isLastMonth, ref DateTime stDate, ref DateTime enDate, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (isThisWeek)
            {
                int dayofweek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                stDate = DateTime.Now.AddDays(0 - dayofweek);
                enDate = DateTime.Now.AddDays(7 - dayofweek);
                stDate = new DateTime(stDate.Year, stDate.Month, stDate.Day, 0, 0, 0);
                enDate = new DateTime(enDate.Year, enDate.Month, enDate.Day, 23, 59, 59);
            }
            else if (isThisMonth)
            {
                stDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                enDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23, 59, 59);
            }
            else if (isLastMonth)
            {
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year;
                int month = currentDate.Month;

                if (month == 1)
                {
                    year = year - 1;
                    month = 12;
                }
                else
                {
                    month = month - 1;
                }
                stDate = new DateTime(year, month, 1, 0, 0, 0);
                enDate = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
            }
            else
            {
                stDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                enDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            }
        }

        public Return Return(int errorId = 100, string errorCode = "E100", bool status = false, string message = "FAILURE", object data = null, int resultId = 0)
        {
            Return rtn = new Return();
            rtn.ErrorId = errorId;
            rtn.ErrorCode = errorCode;
            rtn.Status = status;
            rtn.Message = message;
            rtn.Data = data;
            rtn.ResultID = resultId;
            return rtn;
        }

        public string PhoneNumber(PhoneNumber strPhoneNumber)
        {
            try
            {
                if (strPhoneNumber != null)
                {
                    return strPhoneNumber.ISD + strPhoneNumber.Number;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }
    }
}
