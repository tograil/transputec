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
using System.Net.Http;
using System.Data;
using Newtonsoft.Json;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Core.Users.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.IO;
using CrisesControl.Core.Models;
using System.Linq;
using System.Collections.Generic;
using CrisesControl.SharedKernel.Utils;
using Microsoft.EntityFrameworkCore;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Users;
using CrisesControl.Core.Companies;
using System.Security.Cryptography;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.Core.Exceptions.InvalidOperation;
using System.Text.RegularExpressions;
using CrisesControl.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Twilio;
using Twilio.Rest.Lookups.V1;

namespace CrisesControl.Infrastructure.Repositories
{
    public class DBCommonRepository: IDBCommonRepository
    {
        private readonly CrisesControlContext _context;
        private int userId;
        private int companyId;
        private readonly string timeZoneId = "GMT Standard Time";
        private readonly IHttpContextAccessor _httpContextAccessor;
        // private readonly ISenderEmailService _SDE;
        //private readonly IUserRepository _userRepository;
        ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        bool isretry = false;
        public delegate void UpdateHandler(object sender, UpdateEventArgs e);
        public bool isValidPhone = false;

        public DBCommonRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor /*,ISenderEmailService SDE*/)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            //_SDE = SDE;
        }

        public class UpdateEventArgs : EventArgs
        {
            public UpdateEventArgs(string _Msg)
            {
                Message = _Msg;
            }
            public string Message { get; }
        }
        public bool IsValidPhone
        {
            get => isValidPhone;
            set => isValidPhone = value;
        }

        public async Task<StringBuilder> ReadHtmlFile(string fileCode, string source, int companyId,  string subject, string provider = "AWSSES")
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
                    var content = _context.Set<EmailTemplate>()
                                   .Where(MSG => MSG.Code == fileCode && MSG.CompanyId == 0
                                  )
                                   .Union(_context.Set<EmailTemplate>()
                                          .Where(MSG => MSG.Code == fileCode && MSG.CompanyId == companyId
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
                            var desc = _context.Set<EmailTemplate>()
                                        .Where(MSG => MSG.Code == "DISCLAIMER_TEXT" && MSG.CompanyId == 0
                                        )
                                  .Union(_context.Set<EmailTemplate>()
                                         .Where(MSG => MSG.Code == "DISCLAIMER_TEXT" && MSG.CompanyId == companyId
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

        public async Task<string> LookupWithKey(string key, string Default = "")
        {
            try
            {
                Dictionary<string, string> Globals = CCConstants.GlobalVars;
                if (Globals.ContainsKey(key))
                {
                    return Globals[key];
                }

                var LKP = await _context.Set<SysParameter>()
                           .Where(L => L.Name == key
                          ).FirstOrDefaultAsync();
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

        public async Task<string> UserName(UserFullName strUserName)
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

        public async Task<string> GetCompanyParameter(string key, int companyId, string Default = "", string customerId = "")
        {
            try
            {
                key = key.ToUpper();

                if (companyId > 0)
                {
                    var LKP = await _context.Set<CompanyParameter>()
                               .Where(CP => CP.Name == key && CP.CompanyId == companyId
                               ).FirstOrDefaultAsync();
                    if (LKP != null)
                    {
                        Default = LKP.Value;
                    }
                    else
                    {
                        var LPR = await _context.Set<LibCompanyParameters>()
                                   .Where(CP => CP.Name == key
                                   ).FirstOrDefaultAsync();
                        if (LPR != null)
                        {
                            Default = LPR.Value;
                        }
                        else
                        {
                            Default = await LookupWithKey(key, Default);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(key))
                {
                    var cmp = _context.Set<Company>().Where(w => w.CustomerId == customerId).FirstOrDefault();
                    if (cmp != null)
                    {
                        var LKP = _context.Set<CompanyParameter>()
                                   .Where(CP => CP.Name == key && CP.CompanyId == cmp.CompanyId
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

        public async Task<string[]> CCRoles(bool addKeyHolder = false, bool addUser = false)
        {
            List<string> rolelist = new List<string> { "ADMIN", "SUPERADMIN" };
            if (addKeyHolder)
                rolelist.Add("KEYHOLDER");

            if (addUser)
                rolelist.Add("USER");

            return rolelist.ToArray();
        }

        public async Task<string> getapiversion()
        {
            string tapiversion = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["WebApiVersion"])!;
            return (string.IsNullOrEmpty(tapiversion) ? "" : tapiversion + "/");
        }

        public async Task<string> PWDencrypt(string strPwdString)
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

        public async Task<string> ToCurrency(decimal amount, int points = 2)
        {
            return "&pound;" + amount.ToString("n" + points);
        }

        public async Task<DateTimeOffset> GetDateTimeOffset(DateTime crTime, string timeZoneId = "GMT Standard Time")
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

        public async Task<DateTime> GetLocalTime(string timeZoneId, DateTime? paramTime = null)
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

        public async Task  CreateObjectRelationship(int targetObjectId, int sourceObjectId, string relationName, int companyId, int createdUpdatedBy, string timeZoneId, string relatinFilter = "")
        {
            try
            {
                if (relationName.ToUpper() == GroupType.GROUP.ToGrString()  || relationName.ToUpper() == GroupType.LOCATION.ToGrString())
                {
                    if (targetObjectId > 0)
                    {
                        int newSourceObjectId = 0;

                        var ObjMapId = await _context.Set<ObjectMapping>()
                                       .Include(OBJ => OBJ.Object)
                                       .Where(OBJ => OBJ.Object.ObjectTableName == relationName)
                                       .Select(a => a.ObjectMappingId).FirstOrDefaultAsync();

                        if (sourceObjectId > 0)
                        {
                            newSourceObjectId = sourceObjectId;
                            CreateNewObjectRelation(newSourceObjectId, targetObjectId, ObjMapId, createdUpdatedBy, timeZoneId, companyId);
                        }

                        if (!string.IsNullOrEmpty(relatinFilter))
                        {
                            if (relationName.ToUpper() == GroupType.GROUP.ToGrString())
                            {
                                newSourceObjectId = _context.Set<Core.Groups.Group>().Where(t => t.GroupName == relatinFilter && t.CompanyId == companyId).Select(t => t.GroupId).FirstOrDefault();
                            }
                            else if (relationName.ToUpper() == GroupType.LOCATION.ToGrString())
                            {
                                newSourceObjectId = _context.Set<Location>().Where(t => t.LocationName == relatinFilter && t.CompanyId == companyId).Select(t => t.LocationId).FirstOrDefault();
                            }
                            CreateNewObjectRelation(newSourceObjectId, targetObjectId, ObjMapId, createdUpdatedBy, timeZoneId, companyId);
                        }
                    }
                }
                else if (relationName.ToUpper() == "DEPARTMENT")
                {
                   await UpdateUserDepartment(targetObjectId, sourceObjectId, createdUpdatedBy, companyId, timeZoneId);
                }
            }
            catch (Exception ex)
            {
                throw new RelationNameNotFoundException(companyId, userId);
            }
        }

        public async Task UpdateUserDepartment(int userId, int departmentId, int createdUpdatedBy, int companyId, string timeZoneId)
        {
            try
            {
                var user = await _context.Set<User>().Where(t => t.UserId == userId).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.DepartmentId = departmentId;
                    user.UpdatedBy = createdUpdatedBy;
                    user.UpdatedOn = await GetDateTimeOffset(DateTime.Now, timeZoneId);
                    _context.Update(user);
                   await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new UserNotFoundException(companyId, userId);
            }
        }
        public async Task CreateNewObjectRelation(int sourceObjectId, int targetObjectId, int objMapId, int createdUpdatedBy, string timeZoneId, int companyId)
        {
            try
            {
                bool isAllObjeRelationExist = await _context.Set<ObjectRelation>().Where(t => t.TargetObjectPrimaryId == targetObjectId
                && t.ObjectMappingId != objMapId
                && t.SourceObjectPrimaryId == sourceObjectId).AnyAsync();

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
                        UpdatedOn = await  GetLocalTime(timeZoneId, System.DateTime.Now),
                        ReceiveOnly = false
                    };
                   await _context.AddAsync(tblDepObjRel);
                   await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new DuplicateEntryException("Dublicate Object Relation");
            }
        }

        public async Task<int> AddPwdChangeHistory(int userId, string newPassword)
        {
            try
            {
                PasswordChangeHistory PH = new PasswordChangeHistory();
                PH.UserId = userId;
                PH.LastPassword = newPassword;
                PH.ChangedDateTime = await GetDateTimeOffset(DateTime.Now);
                await _context.AddAsync(PH);
                await _context.SaveChangesAsync();
                return PH.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DateTimeOffset> ConvertToLocalTime(string timezoneId, DateTimeOffset paramTime)
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
        public async Task DeleteScheduledJob(string jobName, string group)
        {
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;
                await _scheduler.DeleteJob(new JobKey(jobName, group));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetTimeZoneVal(int userId)
        {
            try
            {
                string tmpZoneVal = "GMT Standard Time";
                var userInfo = await _context.Set<User>().Include(c => c.Company)
                                .Where(U => U.UserId == userId)
                                .Select(T => new
                                {
                                    UserTimezone = T.Company.StdTimeZone.ZoneLabel
                                }).FirstOrDefaultAsync();
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

        public async Task<string> FixMobileZero(string strNumber)
        {
            strNumber = (strNumber == null) ? string.Empty : Regex.Replace(strNumber, @"\D", string.Empty);
            strNumber = Left(strNumber, 1) == "0" ?  Left(strNumber, strNumber.Length - 1, 1) : strNumber;
            return strNumber;
        }
        public async Task<string> GetTimeZoneByCompany(int companyId)
        {
            try
            {
                string tmpZoneVal = "GMT Standard Time";
                var Companytime = await _context.Set<Company>().Include(st => st.StdTimeZone)
                                   .Where(C => C.CompanyId == companyId)
                                   .Select(T => new
                                   {
                                       CompanyTimezone = T.StdTimeZone.ZoneLabel
                                   }).FirstOrDefaultAsync();
                if (Companytime != null)
                {
                    tmpZoneVal = Companytime.CompanyTimezone;
                }
                return tmpZoneVal;
            }
            catch (Exception ex) { throw ex; }
            return "GMT Standard Time";
        }
        public async Task CreateLog(string level, string message, Exception ex = null, string controller = "", string method = "", int companyId = 0)
        {

            if (level.ToUpper() == "INFO")
            {
                string CreateLog = await LookupWithKey("COLLECT_PERFORMANCE_LOG");
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

        public async Task UpdateLog(string strErrorID, string strErrorMessage, string strControllerName, string strMethodName, int intCompanyId)
        {
            try
            {
               await CreateLog("INFO",  Left(strErrorMessage, 8000), null, strControllerName, strMethodName, intCompanyId);
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
            
                var comp_pp = await _context.Set<CompanyPaymentProfile>().Where(CPP => CPP.CompanyId == companyId).FirstOrDefaultAsync();
                var comp = await _context.Set<Company>().Where(C => C.CompanyId == companyId).FirstOrDefaultAsync();
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
                           await _set_comms_status(companyId, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < -comp_pp.CreditLimit)
                        { //Used the overdraft amount as well, so stop their SMS and Phone
                            comp.CompanyProfile = "STOP_MESSAGING";
                            sendAlert = true;
                           await _set_comms_status(companyId, stopped_comms, false);
                        }
                        else if (comp_pp.CreditBalance < 0 && comp_pp.CreditBalance > -comp_pp.CreditLimit)
                        { //Using the overdraft facility, can still use the system
                            comp.CompanyProfile = "ON_CREDIT";
                            sendAlert = true;
                          await  _set_comms_status(companyId, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < comp_pp.MinimumBalance)
                        { //Less than the minimum balance, just send an alert, can still use the system.
                            comp.CompanyProfile = "LOW_BALANCE";
                            sendAlert = true;
                           await _set_comms_status(companyId, stopped_comms, true);
                        }
                        comp_pp.UpdatedOn = await GetDateTimeOffset(DateTime.Now);
                        _context.Update(comp_pp);
                        await _context.SaveChangesAsync();

                        if (DateTimeOffset.Now.Subtract(LastUpdate).TotalHours < 24)
                        {
                            sendAlert = false;
                        }

                        string CommsDebug = await LookupWithKey("COMMS_DEBUG_MODE");

                        if (sendAlert && CommsDebug == "false")
                        {
                           
                         // await _userRepository.UsageAlert(companyId);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task _set_comms_status(int companyId, List<string> methods, bool status)
        {
            try
            {
               _context.Set<CompanyComm>().Include(CO => CO.CommsMethod)
                .Where(CM => CM.CompanyId == companyId && methods.Contains(CM.CommsMethod.MethodCode)
                ).ToList().ForEach(x => x.ServiceStatus = status);
               await _context.SaveChangesAsync();
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
            var ItemRec = await _context.Set<CompanyPackageItem>().Where(PI => PI.ItemCode == itemCode && PI.CompanyId == companyId).FirstOrDefaultAsync();
            if (ItemRec != null)
            {
                retVal = ItemRec.ItemValue;
            }
            else
            {
                var LibItemRec = await _context.Set<LibPackageItem>().Where(PI => PI.ItemCode == itemCode).FirstOrDefaultAsync();
                retVal = LibItemRec.ItemValue;
            }
            return retVal;
        }
        public async Task<DateTimeOffset> GetNextReviewDate(DateTimeOffset currentDateTime, string frequency)
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
        public async Task<DateTimeOffset> GetNextReviewDate(DateTimeOffset currentReviewDate, int companyID, int reminderCount,  int reminderCounter)
        {
            try
            {
                reminderCounter = 0;
                int reminder1 = 30;
                int reminder2 = 15;
                int reminder3 = 7;

                int.TryParse(await GetCompanyParameter("SOP_DOCUMENT_REMINDER_1", companyID), out reminder1);
                int.TryParse(await GetCompanyParameter("SOP_DOCUMENT_REMINDER_2", companyID), out reminder2);
                int.TryParse(await GetCompanyParameter("SOP_DOCUMENT_REMINDER_3", companyID), out reminder3);

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
        public async Task<bool> verifyLength(string str, int minLength, int maxLength)
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
        public async  Task<bool> IsPropertyExist(dynamic settings, string name)
        {
            return settings.GetType().GetProperty(name) != null;
        }

        public async Task<LatLng> GetCoordinates(string address)
        {
            LatLng lt = new LatLng();
            lt.Lat = "0";
            lt.Lng = "0";
            if (!string.IsNullOrEmpty(address))
            {
                if (address.Length > 5)
                {
                    string apiUrl = await  LookupWithKey("GOOGLE_MAPS_API_URL");

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

                        lt.Lat =  Left(lt.Lat, 15);
                        lt.Lng =  Left(lt.Lng, 15);
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

        public async Task<bool> connectUNCPath(string uncPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "")
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
        public async Task<string> PureAscii(string str, bool keepAccent = false)
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

        public async Task<string> RandomPassword(int length = 8, int complexity = 4)
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
        public async Task RemoveUserObjectRelation(string relationName, int userId, int sourceObjectId, int companyId, int currentUserId, string timeZoneId)
        {
            try
            {
                if (relationName.ToUpper() == "GROUP" || relationName.ToUpper() == "LOCATION")
                {
                    var ObjMapId = await _context.Set<ObjectMapping>().Include(o => o.Object)
                                    .Where(OBJ => OBJ.Object.ObjectTableName == relationName
                                    ).Select(a => a.ObjectMappingId).FirstOrDefaultAsync();

                    var getRelationRec = await _context.Set<ObjectRelation>()
                                          .Where(OR => OR.ObjectMappingId == ObjMapId && OR.TargetObjectPrimaryId == userId &&
                                          OR.SourceObjectPrimaryId == sourceObjectId).FirstOrDefaultAsync();
                    if (getRelationRec != null)
                    {
                        _context.Remove(getRelationRec);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (relationName.ToUpper() == "DEPARTMENT")
                {
                  await  UpdateUserDepartment(userId, 0, currentUserId, companyId, timeZoneId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task RemoveUserDevice(int userId, bool tokenReset = false)
        {
            try
            {
                var devices = await _context.Set<UserDevice>().Where(UD=> UD.UserId == userId).ToListAsync();
                if (!tokenReset)
                {
                    _context.RemoveRange(devices);
                }
                else
                {
                    devices.ForEach(s => s.DeviceToken = "");
                }
               await _context.SaveChangesAsync();
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

                var result = _context.Set<CrisesControl.Core.Messages.Results>().FromSqlRaw("exec Pro_Message_Process_Log_Insert @MessageID, @EventName, @MethodName, @QueueName, @AdditionalInfo",
                      pMessageID, pEventName, pMethodName, pQueueName, pAdditionalInfo).AsEnumerable();

                result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> Getconfig(string key, string defaultVal = "")
        {
            try
            {
                string value =  _context.Database.GetConnectionString();
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
        public async Task<string> FormatMobile(string ISD, string mobile)
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
        public async Task<bool> IsTrue(string boolVal, bool Default = true)
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
        public async Task<int> ChunkString(string str, int chunkSize)
        {
            int result = 1;
            Math.DivRem(str.Length, chunkSize, out result);
            int z = (str.Length / chunkSize) + (result > 0 ? 1 : 0);
            return z;
        }
        public async Task<dynamic> InitComms(string API_CLASS, string apiClass = "", string clientId = "", string clientSecret = "")
        {
            
            try
            {

                int RetryCount = 2;
                int.TryParse(await LookupWithKey(API_CLASS + "_MESSAGE_RETRY_COUNT"), out RetryCount);

                if (string.IsNullOrEmpty(apiClass))
                    apiClass = await LookupWithKey(API_CLASS + "_API_CLASS");

                if (string.IsNullOrEmpty(clientId))
                    clientId = await LookupWithKey(API_CLASS + "_CLIENTID");

                if (string.IsNullOrEmpty(clientSecret))
                    clientSecret = await LookupWithKey(API_CLASS + "_CLIENT_SECRET");

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
               
            }
        }
        public async Task<string> GetValueByIndex(List<string> valueList, int indexVal)
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
                    return await GetValueByIndex(valueList, 0);
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
        public async Task<bool> IsDayLightOn(DateTime thisDate)
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

        public async Task CancelJobsByGroup(string jobGroup)
        {
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler =  schedulerFactory.GetScheduler().Result;
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(jobGroup);
                var jobKeys = _scheduler.GetJobKeys(groupMatcher).Result;
                foreach (var jobKey in jobKeys)
                {

                   await _scheduler.DeleteJob(jobKey);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteOldFiles(string dirName)
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

               await DeleteScheduledJob("SOP_REVIEW_" + sopHeaderId, "REVIEW_REMINDER");

                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "SOP_REVIEW_" + sopHeaderId;
                string taskTrigger = "SOP_REVIEW_" + sopHeaderId;

                var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "REVIEW_REMINDER", typeof(SOPReviewJob));
                jobDetail.JobDataMap["IncidentID"] = incidentId;
                jobDetail.JobDataMap["SOPHeaderID"] = sopHeaderId;

                int Counter = 0;
                DateTimeOffset DateCheck = await GetNextReviewDate(nextReviewDate, companyId, reminderCount,  Counter);
                jobDetail.JobDataMap["Counter"] = Counter;

                var sop_head = await _context.Set<Sopheader>().Where(SH => SH.SopheaderId == sopHeaderId).FirstOrDefaultAsync();
                sop_head.ReminderCount = Counter;
                _context.Update(sop_head);
                await _context.SaveChangesAsync();

                //if(DateCheck.Date >= DateTime.Now.Date && Counter <= 3) {
                if (DateTimeOffset.Compare(DateCheck, await GetDateTimeOffset(DateTime.Now)) >= 0 && Counter <= 3)
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
                    await _scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                {
                    DateTimeOffset newReviewDate = await GetNextReviewDate(nextReviewDate, reviewFrequency);

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
                var syspr = await _context.Set<SysParameter>().Where(SP => SP.Category == "SOCIAL_HANDLE" && SP.Status == 1).ToListAsync();
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

            int SegWarning = await _context.Database.ExecuteSqlRawAsync("SELECT [dbo].[Incident_Segregation](@IncidentID,@UserID,@CompanyID)", pIncidentId, pUserID, pCompanyId);
            return SegWarning;
        }

        public async Task<List<SocialIntegraion>> GetSocialIntegration(int companyId, string accountType)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pAccountType = new SqlParameter("@AccountType", accountType);

                var result = await  _context.Set<SocialIntegraion>().FromSqlRaw("EXEC Pro_Get_Social_Integration @CompanyID, @AccountType", pCompanyID, pAccountType).ToListAsync();
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
                                            .Where(CP => CP.CompanyId == companyID && CP.CompanyParametersId == parameterID
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
                        CompanyParameter.UpdatedOn = await GetDateTimeOffset(DateTime.Now, timeZoneId);
                        CompanyParameter.UpdatedBy = currentUserID;
                        _context.Update(CompanyParameter);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (!string.IsNullOrEmpty(parameterName))
                {
                    var CompanyParameter = await _context.Set<CompanyParameter>()
                                            .Where(CP => CP.CompanyId == companyID && CP.Name == parameterName).FirstOrDefaultAsync();

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
                        CompanyParameter.UpdatedOn =await GetDateTimeOffset(DateTime.Now, timeZoneId);
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
                    var profile = await _context.Set<CompanyPaymentProfile>().Where(CP => CP.CompanyId == companyID).FirstOrDefaultAsync();
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
                var comp_param = await _context.Set<CompanyParameter>().Where(CP => CP.CompanyId == companyId && CP.Name == name).AnyAsync();
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
                        UpdatedOn = await GetDateTimeOffset(DateTime.Now, timeZoneId)
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
        public async Task<bool> OnTrialStatus(string companyProfile, bool currentTrial)
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

        public async Task<Return> Return(int errorId = 100, string errorCode = "E100", bool status = false, string message = "FAILURE", object data = null, int resultId = 0)
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

        public async Task<string> PhoneNumber(PhoneNumber strPhoneNumber)
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
        public async Task<DateTimeOffset> LookupLastUpdate(string Key)
        {
            try
            {

                var LKP = await _context.Set<SysParameter>()
                           .Where(L => L.Name == Key
                           ).FirstOrDefaultAsync();
                if (LKP != null)
                {
                    return LKP.UpdatedOn;
                }
                return new DateTimeOffset();
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
        }
        public async Task<DateTimeOffset> GetCompanyParameterLastUpdate(string Key, int CompanyId)
        {
            try
            {

                var LKP = await _context.Set<CompanyParameter>()
                           .Where(L => L.Name == Key && L.CompanyId == CompanyId).FirstOrDefaultAsync();
                if (LKP != null)
                {
                    return LKP.UpdatedOn;
                }
                return new DateTimeOffset();
            }
            catch (Exception ex)
            {
                throw ex;
                return new DateTimeOffset();
            }
        }
        public async Task<DateTime> DbDate()
        {
            return new DateTime(1900, 01, 01, 0, 0, 0);
        }


        public async Task<string> RetrieveFormatedAddress(string lat, string lng)
        {
            try
            {
                string APIKey = await  LookupWithKey("GOOGLE_LEGACY_API_KEY");
                string baseUri = "https://maps.googleapis.com/maps/api/" +
                          "geocode/xml?latlng={0},{1}&sensor=false&key={2}";
                string requestUri = string.Format(baseUri, lat, lng, APIKey);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(requestUri);

                Task<string> result = client.GetStringAsync(new Uri(requestUri));

                var xmlElm = XElement.Parse(result.Result);

                var status =(from elm in xmlElm.Descendants()
                              where elm.Name == "status"
                              select elm).FirstOrDefault();

                if (status.Value.ToLower() == "ok")
                {

                    var res = (from elm in xmlElm.Descendants()
                               where elm.Name == "formatted_address"
                               select elm).Skip(1).FirstOrDefault();

                    if (res == null)
                    {
                        res = (from elm in xmlElm.Descendants()
                               where elm.Name == "formatted_address"
                               select elm).FirstOrDefault();
                    }

                    if (res.Value != null)
                    {
                        return res.Value.ToString();
                    }
                    else
                    {
                        if (!isretry)
                        {
                            string tryagain = "";
                            int retrycount = 0;
                            while (tryagain == "" && retrycount < 3)
                            {
                                tryagain = await RetrieveFormatedAddress(lat, lng);
                                isretry = true;
                                retrycount++;
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task UpdateUserLocation(int userid,  string latitude, string longitude, string timeZoneId)
        {
            try
            {
                if (!string.IsNullOrEmpty(latitude) && !string.IsNullOrEmpty(longitude) && latitude != "0" && longitude != "0")
                {
                    var updatelocation = _context.Set<User>().Where(user => user.UserId == userid).FirstOrDefault();
                    if (updatelocation != null)
                    {
                        updatelocation.Lat = Left(latitude, 10).Replace(",", ".");
                        updatelocation.Lng = Left(longitude, 10).Replace(",", ".");
                        updatelocation.LastLocationUpdate = await GetDateTimeOffset(DateTime.Now, timeZoneId);
                        _context.Update(updatelocation);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",")
        {
            //DataTable dataTable = new DataTable();
            StringBuilder csvRows = new StringBuilder();
            string row = "";
            int columns;
            try
            {
                //dataTable.Load(dataReader);
                columns = dataTable.Columns.Count;
                //Create Header
                if (includeHeaderAsFirstRow)
                {
                    for (int index = 0; index < columns; index++)
                    {
                        row += (dataTable.Columns[index]);
                        if (index < columns - 1)
                            row += (separator);
                    }
                    row += (Environment.NewLine);
                }
                csvRows.Append(row);

                //Create Rows
                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    row = "";
                    //Row
                    for (int index = 0; index < columns; index++)
                    {
                        string value = dataTable.Rows[rowIndex][index].ToString();

                        //If type of field is string
                        if (dataTable.Rows[rowIndex][index] is string)
                        {
                            //If double quotes are used in value, ensure each are replaced by double quotes.
                            if (value.IndexOf("\"") >= 0)
                                value = value.Replace("\"", "\"\"");

                            //If separtor are is in value, ensure it is put in double quotes.
                            if (value.IndexOf(separator) >= 0)
                                value = "\"" + value + "\"";

                            //If string contain new line character
                            while (value.Contains("\r"))
                            {
                                value = value.Replace("\r", "");
                            }
                            while (value.Contains("\n"))
                            {
                                value = value.Replace("\n", "");
                            }
                        }
                        row += value;
                        if (index < columns - 1)
                            row += separator;
                    }
                    dataTable.Rows[rowIndex][columns - 1].ToString().ToString().Replace(separator, " ");
                    row += Environment.NewLine;
                    csvRows.Append(row);
                }
                dataTable.Dispose();
                return csvRows.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task ModelInputLog(string controllerName, string methodName, int userID, int companyID, dynamic data)
        {
            try
            {

                string json = JsonConvert.SerializeObject(data);

                var pControllerName = new SqlParameter("@ControllerName", controllerName);
                var pMethodName = new SqlParameter("@MethodName", methodName);
                var pUserID = new SqlParameter("@UserID", userID);
                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                var pData = new SqlParameter("@Data", json);

               await _context.Database.ExecuteSqlRawAsync("Pro_Log_Model_Data @ControllerName, @MethodName, @UserID, @CompanyID, @Data",
                pControllerName, pMethodName, pUserID, pCompanyID, pData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DateTimeOffset> ToNullIfTooEarlyForDb(DateTimeOffset date, bool convertUTC = false)
        {
            DateTimeOffset retDate = (date.Year >= 1990) ? date : DateTime.Now;
            if (!convertUTC)
            {
                return retDate;
            }
            else
            {
                //retDate = GetLocalTimeScheduler("GMT Standard Time", retDate);
                retDate = await GetDateTimeOffset(retDate.LocalDateTime);
            }
            return retDate;
        }
        public string LogWrite(string str, string strType = "I")
        {
            return (strType == "I" ? "Info: " : "Error: ") + str + Environment.NewLine;
        }

        public async Task<bool> AddUserTrackingDevices(int userID, int messageListID = 0)
        {
            var devices = await _context.Set<UserDevice>().Where(UD => UD.UserId == userID).ToListAsync();
            if (devices != null)
            {
                foreach (var device in devices)
                {
                    if (device.DeviceType.ToUpper().Contains("ANDROID") || device.DeviceType.ToUpper().Contains("WINDOWS"))
                        await AddTrackingDevice(device.CompanyId, device.UserDeviceId, device.DeviceId, device.DeviceType, messageListID);

                }
                return true;
            }
            return false;
        }

        public async Task AddTrackingDevice(int companyID, int userDeviceID, string deviceAddress, string deviceType, int messageListID = 0)
        {
            try
            {
                MessageDevice MessageDev = new MessageDevice();
                MessageDev.CompanyId = companyID;
                MessageDev.MessageId = 0;
                MessageDev.MessageListId = messageListID;
                MessageDev.UserDeviceId = userDeviceID;
                MessageDev.Method = "PUSH";
                MessageDev.AttributeId = 0;
                MessageDev.MessageText = "Track Device";
                MessageDev.Priority = 100;
                MessageDev.Attempt = 0;
                MessageDev.Status = "PENDING";
                MessageDev.SirenOn = false;
                MessageDev.OverrideSilent = false;
                MessageDev.CreatedOn = await GetDateTimeOffset(DateTime.Now);
                MessageDev.UpdatedOn =await GetDateTimeOffset(DateTime.Now);
                MessageDev.CreatedBy = 0;
                MessageDev.UpdatedBy = 0;
                MessageDev.DateSent = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                MessageDev.DateDelivered = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                MessageDev.DeviceAddress = deviceAddress;
                MessageDev.DeviceType = deviceType;
                await _context.AddAsync(MessageDev);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task GetFormatedNumber(string inputNumber,  string isdCode, string phoneNum, string defaultISD = "44")
        {

            phoneNum = Regex.Replace(inputNumber, @"\D", string.Empty);
            isdCode = defaultISD;
            IsValidPhone = false;

            if (Left(inputNumber, 1) != "+")
                inputNumber = isdCode + inputNumber;

            try
            {
                string ClientId =await LookupWithKey("TWILIO_CLIENTID");
                string ClientSecret =await LookupWithKey("TWILIO_CLIENT_SECRET");
                TwilioClient.Init(ClientId, ClientSecret);

                var phoneNumber = PhoneNumberResource.Fetch(pathPhoneNumber: new Twilio.Types.PhoneNumber(inputNumber));
                if (phoneNumber != null)
                {
                    string countryCode = phoneNumber.CountryCode;
                    phoneNum = Regex.Replace(phoneNumber.NationalFormat, @"\D", string.Empty).TrimStart('0');
                    IsValidPhone = true;
                    var isd_rec =await  _context.Set<Country>().Where(w => w.Iso2code == countryCode).FirstOrDefaultAsync();
                    if (isd_rec != null)
                    {
                        isdCode = Left(isd_rec.CountryPhoneCode, 1) != "+" ? "+" + isd_rec.CountryPhoneCode : isd_rec.CountryPhoneCode;
                    }
                }
                else
                {
                    phoneNum = Regex.Replace(inputNumber, @"\D", string.Empty);
                    IsValidPhone = false;
                }
            }
            catch (Exception ex)
            {
                IsValidPhone = false;
            }
        }
        public async Task AddUserInvitationLog(int companyId, int userID, string actionType, int currentUserId, string timeZoneId)
        {
            try
            {
                DateTimeOffset dtNow =await GetDateTimeOffset(DateTime.Now, timeZoneId);

                var pCompanyId = new SqlParameter("@CompanyID", companyId);
                var pUserID = new SqlParameter("@UserID", userID);
                var pActionType = new SqlParameter("@ActionType", actionType);
                var pActionDate = new SqlParameter("@ActionDate", dtNow);
                var pCreatedBy = new SqlParameter("@CreatedBy", currentUserId);

               await _context.Database.ExecuteSqlRawAsync("Pro_Add_User_InviationLog @CompanyID,@UserID,@ActionType,@ActionDate,@CreatedBy", pCompanyId, pUserID, pActionType, pActionDate, pCreatedBy);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteRecording(string recordingID)
        {
            try
            {
                bool SendInDirect = await IsTrue(await LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                string TwilioRoutingApi = await LookupWithKey("TWILIO_ROUTING_API");

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                CommsAPI.DeleteRecording(recordingID);
            }
            catch (Exception)
            {

            }
        }


    }
}
