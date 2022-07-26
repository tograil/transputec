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
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Location = CrisesControl.Core.Locations.Location;
using log4net;

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
                            var desc = (from MSG in  _context.Set<EmailTemplate>()
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
                                newSourceObjectId = _context.Set<Core.Groups.Group>().Where(t=>t.GroupName == relatinFilter && t.CompanyId == companyId).Select(t=>t.GroupId).FirstOrDefault();
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
            catch (Exception ex) {
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
            catch (Exception ex) {
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
        public async Task GetSetCompanyComms(int CompanyID)
        {
            try
            {
                DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
                var comp_pp = await  _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == CompanyID).FirstOrDefaultAsync();
                var comp = await _context.Set<Company>().Where(C=> C.CompanyId == CompanyID).FirstOrDefaultAsync();
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
                            _set_comms_status(CompanyID, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < -comp_pp.CreditLimit)
                        { //Used the overdraft amount as well, so stop their SMS and Phone
                            comp.CompanyProfile = "STOP_MESSAGING";
                            sendAlert = true;
                            _set_comms_status(CompanyID, stopped_comms, false);
                        }
                        else if (comp_pp.CreditBalance < 0 && comp_pp.CreditBalance > -comp_pp.CreditLimit)
                        { //Using the overdraft facility, can still use the system
                            comp.CompanyProfile = "ON_CREDIT";
                            sendAlert = true;
                            _set_comms_status(CompanyID, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < comp_pp.MinimumBalance)
                        { //Less than the minimum balance, just send an alert, can still use the system.
                            comp.CompanyProfile = "LOW_BALANCE";
                            sendAlert = true;
                            _set_comms_status(CompanyID, stopped_comms, true);
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
                            await SDE.UsageAlert(CompanyID);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void _set_comms_status(int CompanyId, List<string> methods, bool status)
        {
            try
            {
                (from CM in _context.Set<CompanyComm>().AsEnumerable()
                 join CO in _context.Set<CommsMethod>().AsEnumerable() on CM.MethodId equals CO.CommsMethodId
                 where CM.CompanyId == CompanyId && methods.Contains(CO.MethodCode)
                 select CM).ToList().ForEach(x => x.ServiceStatus = status);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> GetPackageItem(string ItemCode, int CompanyId)
        {
            string retVal = string.Empty;
            ItemCode = ItemCode.Trim();
            var ItemRec = await  _context.Set<CompanyPackageItem>().Where(PI=> PI.ItemCode == ItemCode && PI.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (ItemRec != null)
            {
                retVal = ItemRec.ItemValue;
            }
            else
            {
                var LibItemRec = await _context.Set<LibPackageItem>().Where(PI=> PI.ItemCode == ItemCode).FirstOrDefaultAsync();
                retVal = LibItemRec.ItemValue;
            }
            return retVal;
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
                throw ex;
                ReminderCounter = 0;
                return CurrentReviewDate.AddYears(-1).Date;
            }
        }

    }
}
