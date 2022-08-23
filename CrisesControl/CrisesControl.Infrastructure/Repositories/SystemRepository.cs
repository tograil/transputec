using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Models;
using CrisesControl.Core.System;
using CrisesControl.Core.System.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using MessageMethod = CrisesControl.SharedKernel.Enums.MessageMethod;

namespace CrisesControl.Infrastructure.Repositories
{
    public class SystemRepository : ISystemRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<SystemRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DBCommon DBC;
        public SystemRepository(CrisesControlContext context, ILogger<SystemRepository> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._logger = logger;
            this._httpContextAccessor = httpContextAccessor;
            DBC = new DBCommon(_context, _httpContextAccessor);
        }
        public async Task<string> ExportTrackingData(int TrackMeID, int UserDeviceID, DateTimeOffset StartDate, DateTimeOffset EndDate, int OutUserCompanyId)
        {
            var pTrackMeID = new SqlParameter("@TrackMeID", TrackMeID);
            var pUserDeviceID = new SqlParameter("@UserDeviceID", UserDeviceID);
            var pStartDate = new SqlParameter("@StartDate", StartDate);
            var pEndDate = new SqlParameter("@EndDate", EndDate);



            string FileName = "Tracking_" + TrackMeID + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".csv";

            string ResultFilePath = DBC.Getconfig("ImportResultPath");
            string ExportPath = ResultFilePath + OutUserCompanyId + "\\DataExport\\";
            string FilePath = ExportPath + FileName;

            if (!Directory.Exists(ExportPath))
            {
                Directory.CreateDirectory(ExportPath);
                DBC.DeleteOldFiles(ExportPath);
            }
            string headerRow = string.Empty;

            var ExportData = await _context.Set<TrackingExport>().FromSqlRaw("exec Pro_ExportTrackingData @TrackMeID, @UserDeviceID,@StartDate,@EndDate",
                    pTrackMeID, pUserDeviceID, pStartDate, pEndDate).ToListAsync();

            using (StreamWriter SW = new StreamWriter(FilePath))
            {

                headerRow = string.Format("\"{0}\",\"{1}\",\"{2}\"", "Time Stamp", "Latitude", "Longitude");

                SW.WriteLine(headerRow);

                foreach (var row in ExportData)
                {
                    string rowdata = string.Format("\"{0}\",\"{1}\",\"{2}\"", row.CreatedOn, row.Latitude, row.Longitude);
                    if (!string.IsNullOrEmpty(rowdata))
                        SW.WriteLine(rowdata);
                }
            }
            return FileName;
        }
        public async Task<List<ModelLogReturn>> GetModelLog(DateTimeOffset startDate, DateTimeOffset endDate, int recordStart, int recordLength, string searchString, string orderBy, string orderDir)
        {
            try
            {

                var pStartDate = new SqlParameter("@StartDate", startDate);
                var pEndDate = new SqlParameter("@EndDate", endDate);
                var pRecordStart = new SqlParameter("@RecordStart", recordStart);
                var pRecordLength = new SqlParameter("@RecordLength", recordLength);
                var pSearchString = new SqlParameter("@SearchString", searchString);
                var pOrderBy = new SqlParameter("@OrderBy", orderBy);
                var pOrderDir = new SqlParameter("@OrderDir", orderDir);


                var LogData = await _context.Set<ModelLogReturn>().FromSqlRaw("exec Pro_Get_Model_Log @StartDate, @EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir",
                        pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir).ToListAsync();

                return LogData;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<AuditHelp>> GetAuditLogsByRecordId(string tableName, int recordId, bool isThisWeek, bool isThisMonth, bool isLastMonth,
           DateTimeOffset startDate, DateTimeOffset endDate, bool limitResult, int companyId)
        {

            try
            {
                int loglimit = 100;
                int.TryParse(DBC.LookupWithKey("ITEM_AUDIT_LOG_LIMIT"), out loglimit);

                if (!limitResult)
                    loglimit = 1000000;

                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;

                DBC.GetStartEndDate(isThisWeek, isThisMonth, isLastMonth, ref stDate, ref enDate, startDate, endDate);

                var pRecordId = new SqlParameter("@RecordID", recordId);
                var pCompanyId = new SqlParameter("@CompanyID", companyId);
                var pLogLimit = new SqlParameter("@LogLimit", loglimit);
                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);

                List<AuditHelp> ListAudit = new List<AuditHelp>();

                #region Users
                //Use: EXEC [dbo].[Pro_Audit_GetUserData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole User Block)
                if (tableName.ToUpper().Trim() == "USERS")
                {

                    var Udata = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetUserData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                         pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in Udata)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion Users
                #region Incident
                //Use: EXEC [dbo].[Pro_Audit_GetIncidentData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole INCIDENT Block)
                if (tableName.ToUpper().Trim() == "INCIDENT")
                {

                    var Idata = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetIncidentData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                       pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in Idata)
                    {

                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion 3
                #region Location
                //Use: EXEC [dbo].[Pro_Audit_GetLocationData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole LOCATION Block)
                if (tableName.ToUpper().Trim() == "LOCATION")
                {

                    var Ldata = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetLocationData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                       pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in Ldata)
                    {

                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion 4
                #region Group
                //Use: EXEC [dbo].[Pro_Audit_GetGroupData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole GROUP Block)
                if (tableName.ToUpper().Trim() == "GROUP")
                {

                    var Ddata = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetGroupData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                      pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in Ddata)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion 5
                #region Department
                //Use: EXEC [dbo].[Pro_Audit_GetGroupData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole GROUP Block)
                if (tableName.ToUpper().Trim() == "DEPARTMENT")
                {

                    var Ddata = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetDepartmentData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                      pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in Ddata)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString == "REMOVED" ? DBC.UserName(logItem.RecordUserName) + " has been removed from department" : logItem.EventString == "ADDED" ? DBC.UserName(logItem.RecordUserName) + " added to department" : logItem.EventString
                        });
                    }
                }
                #endregion 5
                #region Security Group
                //Use: EXEC [dbo].[Pro_Audit_GetSecurityGroupData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole SECURITY GROUP Block)
                if (tableName.ToUpper().Trim() == "SECURITYGROUP")
                {
                    var data = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetSecurityGroupData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                      pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in data)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion Security Group
                #region Company
                //Use: EXEC [dbo].[Pro_Audit_GetCompanyData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole COMPANY Block)
                if (tableName.ToUpper().Trim() == "COMPANY")
                {
                    var data = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetCompanyData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                      pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in data)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion Company
                #region Scheduler
                //Use: EXEC [dbo].[Pro_Audit_GetSchedulerData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole SCHEDULER Block)
                if (tableName.ToUpper().Trim() == "SCHEDULER")
                {
                    var data = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetSchedulerData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                     pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in data)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion Scheduler
                #region ExTrigger
                //Use: EXEC [dbo].[Pro_Audit_GetExTriggerData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole EXTRIGGER Block)
                if (tableName.ToUpper().Trim() == "EXTRIGGER")
                {

                    var data = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetExTriggerData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                      pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in data)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion ExTrigger
                #region Assets
                //Use: EXEC [dbo].[Pro_Audit_GetAssetData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole ASSETS Block)
                if (tableName.ToUpper().Trim() == "ASSETS")
                {

                    var data = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetAssetData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                      pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in data)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion Assets
                #region Failed Login
                //Use: EXEC [dbo].[Pro_Audit_GetAssetData] @RecordID,@CompanyID,@LogLimit,@StartDate,@EndDate (for the whole ASSETS Block)
                if (tableName.ToUpper().Trim() == "LOGINFALIURE")
                {

                    var data = await _context.Set<AuditList>().FromSqlRaw("exec Pro_Audit_GetLoginFailureData @RecordID, @CompanyID, @LogLimit, @StartDate, @EndDate",
                      pRecordId, pCompanyId, pLogLimit, pStartDate, pEndDate).ToListAsync();

                    foreach (var logItem in data)
                    {
                        ListAudit.Add(new AuditHelp()
                        {
                            UserName = logItem.UserName,
                            EventDate = logItem.EventDateUTC,
                            EventStatement = logItem.EventString
                        });
                    }
                }
                #endregion Failed Login
                return ListAudit;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> ExportCompanyData(int outUserCompanyId, string entity, int outLoginUserId, bool showDeleted = false)
        {

            DataTable dt = new DataTable();
            try
            {
                //var pCompanyId = new SqlParameter("@CompanyID", OutUserCompanyId);
                //var pUserID = new SqlParameter("@UserID", OutLoginUserId);
                //var pShowDeleted = new SqlParameter("@ShowDeleted", InputModel.ShowDeleted);

                string FileName = entity + "_" + DateTime.Now.ToString("ddMMyyhhmmss") + ".csv";

                string ResultFilePath = DBC.Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + outUserCompanyId + "\\DataExport\\";
                string FilePath = ExportPath + FileName;

                DBC.connectUNCPath();

                if (!Directory.Exists(ExportPath))
                {
                    Directory.CreateDirectory(ExportPath);
                    DBC.DeleteOldFiles(ExportPath);
                }

                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }

                string headerRow = string.Empty;
                string ReportSP = "";

                List<ReportParam> rparams = new List<ReportParam>();
                if (entity.ToUpper() == "USER")
                {
                    ReportSP = "GetAllUserDetails_ByCompany";
                   
                }
                else if (entity.ToUpper() == "LOCATION")
                {
                    ReportSP = "Pro_ExportLocation";
                    
                }
                else if (entity.ToUpper() == "GROUP")
                {
                    ReportSP = "Pro_ExportGroup";
                
                }
                else if (entity.ToUpper() == "DEPARTMENT")
                {
                    ReportSP = "Pro_ExportDepartment";

                }
                else if (entity.ToUpper() == "DEVICES")
                {
                    ReportSP = "Pro_ExportDeviceList";

                }

                string constr = _context.Database.GetConnectionString();
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand(ReportSP + " @CompanyID, @UserID, @ShowDeleted"))
                    {
                        cmd.Parameters.AddWithValue("@CompanyID", outUserCompanyId);
                        cmd.Parameters.AddWithValue("@UserID", outLoginUserId);
                        cmd.Parameters.AddWithValue("@ShowDeleted", showDeleted);

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Connection = con;
                            con.Open();
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                        }
                    }
                }


                var data = DBC.ToCSVHighPerformance(dt, true, ",");

                File.WriteAllText(@FilePath, data);

                if (File.Exists(@FilePath))
                {
                    return FileName;
                }
                return "File not found or created" + FilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<HttpResponseMessage> DownloadExportFile(int companyId, string fileName)
        {
            try
            {
                string MethodName = "DownloadExportFile";
                string ResultFilePath = DBC.Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + companyId + "\\DataExport\\";
                string FilePath = ExportPath + fileName;
                var badresult = new HttpResponseMessage(HttpStatusCode.NotFound);

                if (File.Exists(FilePath))
                {
                    var stream = new MemoryStream(await File.ReadAllBytesAsync(FilePath), 0, File.ReadAllBytes(FilePath).Length, true, true);

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(stream.GetBuffer())
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    return result;
                }
                else
                {
                    DBC.UpdateLog("0", fileName, "SystemContoller", MethodName, companyId);
                    return badresult;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<HttpResponseMessage> CompanyStatsAdmin(int outUserCompanyId)
        {
            string MethodName = "CompanyStatsAdmin";

            var badresult = new HttpResponseMessage(HttpStatusCode.NotFound);

            try
            {


                string FileName = "CompanyStats_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".csv";

                string ResultFilePath = DBC.Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + outUserCompanyId + "\\DataExport\\";
                string FilePath = ExportPath + FileName;

                if (!Directory.Exists(ExportPath))
                {
                    Directory.CreateDirectory(ExportPath);
                    DBC.DeleteOldFiles(ExportPath);
                }
                string headerRow = string.Empty;

                var ExportData = await _context.Set<AdminCompanyStats>().FromSqlRaw("exec Pro_Report_Admin_CompanyStats").ToListAsync();

                using (StreamWriter SW = new StreamWriter(FilePath))
                {

                    headerRow = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"",
                        "CompanyId", "Company", "Company Status", "Active Staff", "InActive", "Deleted", "Pending Verification", "Staff Pending Verification",
                        "KeyHolder Pending Verification", "Admin Pending Verification", "Total Active Admin", "Total Active KeyHolder");

                    SW.WriteLine(headerRow);

                    foreach (var row in ExportData)
                    {
                        string rowdata = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"",
                            row.CompanyId, row.Company, row.CompanyStatus, row.ActiveStaff, row.InActive, row.Deleted, row.PendingVerification, row.StaffPendingVerification,
                            row.KeyHolderPendingVerification, row.AdminPendingVerification, row.TotalActiveAdmin, row.TotalActiveKeyHolder);
                        if (!string.IsNullOrEmpty(rowdata))
                            SW.WriteLine(rowdata);
                    }
                }

                if (File.Exists(FilePath))
                {
                    var stream = new MemoryStream(File.ReadAllBytes(FilePath), 0, File.ReadAllBytes(FilePath).Length, true, true);

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(stream.GetBuffer())
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = FileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    return result;
                }
                else
                {
                    DBC.UpdateLog("0", FileName, "SystemController", MethodName, outUserCompanyId);
                    return badresult;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ErrorLogReturn>> GetErrorLog(DateTimeOffset startDate, DateTimeOffset endDate, int recordStart, int recordLength, string searchString, string orderBy, string orderDir)
        {
            try
            {

                var pStartDate = new SqlParameter("@StartDate", startDate);
                var pEndDate = new SqlParameter("@EndDate", endDate);
                var pRecordStart = new SqlParameter("@RecordStart", recordStart);
                var pRecordLength = new SqlParameter("@RecordLength", recordLength);
                var pSearchString = new SqlParameter("@SearchString", searchString);
                var pOrderBy = new SqlParameter("@OrderBy", orderBy);
                var pOrderDir = new SqlParameter("@OrderDir", orderDir);


                var LogData = await _context.Set<ErrorLogReturn>().FromSqlRaw("exec Pro_Get_Error_Log @StartDate, @EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir",
                        pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir).ToListAsync();
                if (LogData != null)
                {
                    return LogData;
                }
                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<HttpResponseMessage> ApiStatus()
        {

            string MethodName = "ApiStatus";

            var result = new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                var dbresult = await _context.Set<Country>().ToListAsync();

                if (dbresult != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);//"DATABASE_QUERY_FAILED"
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> TwilioLogDump( string logType, List<CallResource> calls, List<MessageResource> texts, List<RecordingResource> recordings)
        {
            try
            {

                CommsLogsHelper CLH = new CommsLogsHelper();
                string session = Guid.NewGuid().ToString();

                if (logType == LogType.PHONE.ToLGString())
                {
                    foreach (var item in calls)
                    {
                        CLH.ProcessCallLogs(item, session);
                    }
                }
                else if (logType == LogType.TEXT.ToLGString())
                {
                    foreach (var item in texts)
                    {
                        CLH.ProcessSMSLog(item, session);
                    }
                }
                else if (logType == LogType.RECORDING.ToLGString())
                {
                    foreach (var item in recordings)
                    {
                        CLH.ProcessRecLog(item, session);
                    }
                }
                var result = CLH.CreateCommsQueueSession(session);
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> PushTwilioLog(string method, string sId)
        {
            try
            {

                string PHONESID = DBC.LookupWithKey("PHONESID");
                string PHONETOKEN = DBC.LookupWithKey("PHONETOKEN");

                string session = Guid.NewGuid().ToString();

                CommsLogsHelper CLH = new CommsLogsHelper();

                TwilioClient.Init(PHONESID, PHONETOKEN);

                if (method.ToUpper() == MessageType.Text.ToDbString())
                {
                    var message = MessageResource.Fetch(pathSid: sId);
                    if (message != null)
                    {
                        CLH.ProcessSMSLog(message, session);
                    }
                }
                else if (method.ToUpper() == MessageType.Phone.ToDbString())
                {
                    var call = CallResource.Fetch(pathSid: sId);
                    if (call != null)
                    {
                        CLH.ProcessCallLogs(call, session);
                    }
                }

                var result = CLH.CreateCommsQueueSession(session);
                if (result)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> PushCMLog(string method, string sId)
        {
            try
            {

                string PHONESID = DBC.LookupWithKey("PHONESID");
                string PHONETOKEN = DBC.LookupWithKey("PHONETOKEN");

                string session = Guid.NewGuid().ToString();

                CommsLogsHelper CLH = new CommsLogsHelper();

                TwilioClient.Init(PHONESID, PHONETOKEN);

                if (method.ToUpper() == MessageType.Text.ToDbString().ToUpper())
                {
                    var message = MessageResource.Fetch(pathSid: sId);
                    if (message != null)
                    {
                        CLH.ProcessSMSLog(message, session);
                    }
                }
                else if (method.ToUpper() == MessageType.Phone.ToDbString())
                {
                    var call = CallResource.Fetch(pathSid: sId);
                    if (call != null)
                    {
                        CLH.ProcessCallLogs(call, session);
                    }
                }

                var result = CLH.CreateCommsQueueSession(session);
                if (result)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CleanLoadTestResult()
        {
            try
            {
                

                  await  _context.Database.ExecuteSqlRawAsync("System_Clean_Load_Testing_Result");

             
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
