using AutoMapper;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Core.Reports.SP_Response;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class ReportRepository : IReportsRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<ReportRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        private int UserID;
        private int CompanyID;

        public ReportRepository(CrisesControlContext context,
                                IHttpContextAccessor httpContextAccessor,
                                ILogger<ReportRepository> logger,
                                IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<SOSItem>> GetSOSItems()
        {
            try
            {
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

                var pUserId = new SqlParameter("@UserID", UserID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);

                var result = await _context.Set<SOSItem>().FromSqlRaw("exec Pro_Get_SOS_Alerts {0},{1}", pCompanyID, pUserId).ToListAsync();

                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<IncidentPingStatsCount>> GetIncidentPingStats(int CompanyID, int NoOfMonth)
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pNoOfMonth = new SqlParameter("@NoOfMonth", NoOfMonth);

                var result = await _context.Set<IncidentPingStatsCount>().FromSqlRaw("exec Pro_Report_Dashboard_Incident_Ping_Stats_ByMonth {0},{1}", pCompanyID, pNoOfMonth).ToListAsync();
                return result;


            }
            catch (Exception ex)
            {
            }
            return new List<IncidentPingStatsCount>();
        }

        public async Task<DataTablePaging> GetIndidentMessageAck(int MessageId, int MessageAckStatus, int MessageSentStatus, int RecordStart, int RecordLength, string search,
             string OrderBy, string OrderDir, int draw, string Filters, string CompanyKey, string Source="WEB")
        {
            try
            {
                const string ord = "PrimaryEmail";
                const string dir = "asc";
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                var SearchString = (search != null) ? search: string.Empty;
                
                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = ord;

                if (string.IsNullOrEmpty(OrderDir))
                    OrderDir = dir;

                var pMessageId = new SqlParameter("@MessageID", MessageId);
                var pMessageAckStatus = new SqlParameter("@MessageAckStatus", MessageAckStatus);
                var pMessageSentStatus = new SqlParameter("@MessageSentStatus", MessageSentStatus);
                var pUserID = new SqlParameter("@UserID", UserID);
                var pSource = new SqlParameter("@Source", Source);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pFilters = new SqlParameter("@Filters", Filters);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

                var ackList = await  _context.Set<MessageAcknowledgements>().FromSqlRaw("exec Pro_Get_Message_Acknowledgements @MessageID, @MessageAckStatus, @MessageSentStatus, @UserID,@Source, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@Filters,@UniqueKey",
                    pMessageId, pMessageAckStatus, pMessageSentStatus, pUserID, pSource, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pFilters, pUniqueKey).ToListAsync();
                int totalRecord = 0;
                totalRecord = ackList.Count;

                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = draw;
                rtn.RecordsTotal = totalRecord;
                rtn.RecordsFiltered = ackList.Count;
                rtn.Data = ackList;


                return rtn;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return null;
            }
        }

        public async Task<List<ResponseSummary>> ResponseSummary(int MessageID)
        {
            try {
                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var result = await _context.Set<ResponseSummary>().FromSqlRaw("exec Pro_Get_Message_Acknowledgements_Responses @MessageID", pMessageID).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {

                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return new List<ResponseSummary>();
        }

        public async Task<List<DataTablePaging>> GetIndidentMessageNoAck(int draw, int IncidentActivationId, int RecordStart, int RecordLength, string? SearchString, string? UniqueKey)
        {
            try
            {
                const string OrderBy = "UserId";
                const string OrderDir = "asc";
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
                var pIncidentActivationId = new SqlParameter("@ActiveIncidentID", IncidentActivationId);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                 var pUserID = new SqlParameter("@UserID", UserID);
                 var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pUniqueKey = new SqlParameter("@UniqueKey", UniqueKey);

                var result = await _context.Set<MessageAcknowledgements>().FromSqlRaw("exec Pro_Get_No_Ack_Users @ActiveIncidentID, @UserID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                   pIncidentActivationId, pUserID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey).ToListAsync();

                 int totalRecord = 0;
                totalRecord = result.Count;

               List<DataTablePaging> rtn = new List<DataTablePaging>(await _context.Set<DataTablePaging>().Where(x=>x.Draw== draw).Select(n=>
                new DataTablePaging() { 
                Draw = draw,
                RecordsTotal = totalRecord,
                RecordsFiltered = result.Count,
                Data = result
                }    ).ToListAsync());
                return rtn;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                          ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
               
            }
            return new List<DataTablePaging>();


        }

        public CurrentIncidentStatsResponse GetCurrentIncidentStats(string timeZoneId)
        {
            DateTimeOffset baseDate = GetLocalTime(timeZoneId, DateTime.Now.Date);
            var today = baseDate;
            var startDate = GetLocalTime(timeZoneId, DateTime.Now.Date.AddDays(1 - DateTime.Now.Day));

            var pStartDate = new SqlParameter("@StartDate", startDate);
            var pEndDate = new SqlParameter("@EndDate", today);
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var result = _context.Set<CurrentIncidentStatsResponse>().FromSqlRaw("Pro_Report_GetCurrentIncidentStats @StartDate, @EndDate, @CompanyID",
                pStartDate,
                pEndDate,
                pCompanyID).ToList()?.FirstOrDefault();
            return result;
        }
        
        public IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId)
        {
            var pIncidentActivationID = new SqlParameter("@IncidentActivationID", incidentActivationId);
            var pCompanyID = new SqlParameter("@CompanyID", companyId);
            var pUserID = new SqlParameter("@UserID", userId);
            var incidentReportData = _context.Set<IncidentData>().FromSqlRaw(
                "Pro_Report_GetIncidentData_ByActivationRef @IncidentActivationID, @CompanyID, @UserID",
                pIncidentActivationID,
                pCompanyID, pUserID).FirstOrDefault();
            if (incidentReportData != null)
            {
                var pIncidentActivationID2 = new SqlParameter("@IncidentActivationID", incidentActivationId);
                var pCompanyID2 = new SqlParameter("@CompanyID", companyId);
                var spKeyContacts = _context.Set<IncidentDataByActivationRefKeyContactsResponse>().FromSqlRaw(
                    "Pro_Report_GetIncidentData_ByActivationRef_KeyContacts @IncidentActivationID, @CompanyID",
                    pIncidentActivationID2,
                    pCompanyID2).ToList();
                List<KeyContact> keyContacts = _mapper.Map<List<IncidentDataByActivationRefKeyContactsResponse>, List<KeyContact>>(spKeyContacts);
                incidentReportData.KeyContacts = keyContacts;

                var pIncidentActivationID3 = new SqlParameter("@IncidentActivationID", incidentActivationId);
                var pCompanyID3 = new SqlParameter("@CompanyID", companyId);
                var spIncidentAssets = _context.Set<IncidentDataByActivationRefIncidentAssetsResponse>().FromSqlRaw(
                    "Pro_Report_GetIncidentData_ByActivationRef_IncidentAssets @IncidentActivationID, @CompanyID",
                    pIncidentActivationID3,
                    pCompanyID3).ToList();
                List<IncidentAssets> incidentAssets = _mapper.Map<List<IncidentDataByActivationRefIncidentAssetsResponse>, List<IncidentAssets>>(spIncidentAssets);
                incidentReportData.IncidentAssets = incidentAssets;
            }
            return incidentReportData;
        }

        public async Task<List<PingGroupChartCount>> GetPingReportChart(DateTime StartDate, DateTime EndDate, int GroupID, string MessageType, int ObjectMappingID)
        {
            try
            {
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

                var pStartDate = new SqlParameter("@StartDate", StartDate);
                    var pEndDate = new SqlParameter("@EndDate", EndDate);
                    var pGroupID = new SqlParameter("@GroupID", GroupID);
                    var pMessageType = new SqlParameter("@MessageType", MessageType);
                    var pObjectMappingID = new SqlParameter("@ObjectMappingID", ObjectMappingID);
                    var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                    var pUserID = new SqlParameter("@UserID", UserID);

                    var result = await _context.Set<PingGroupChartCount>().FromSqlRaw("exec Pro_Report_Ping_Chart @StartDate, @EndDate, @GroupID, @MessageType, @ObjectMappingID, @CompanyID,@UserID",
                    pStartDate, pEndDate, pGroupID, pMessageType, pObjectMappingID, pCompanyID, pUserID).ToListAsync();
                   


                    return result;
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occure while trying to seeding into the database {0},{1},{2}",ex.Message, ex.InnerException, ex.StackTrace);
                
            }
            return new List<PingGroupChartCount>();
        }
        
        public async Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "")
        {
            try
            {
                Key = Key.ToUpper();

                if (CompanyId > 0)
                {
                    var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
                    if (LKP != null)
                    {
                        Default = LKP.Value;
                    }
                    else
                    {

                        var LPR = await _context.Set<LibCompanyParameter>().Where(CP => CP.Name == Key).FirstOrDefaultAsync();
                        if (LPR != null)
                        {
                            Default = LPR.Value;
                        }
                        else
                        {
                            Default = await LookupWithKey(Key, Default);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(CustomerId) && !string.IsNullOrEmpty(Key))
                {

                    var cmp = await _context.Set<Company>().Where(w => w.CustomerId == CustomerId).FirstOrDefaultAsync();
                    if (cmp != null)
                    {
                        var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
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
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                      ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return Default;
            }
        }
                
        public async Task<List<DeliveryOutput>> GetMessageDeliveryReport(DateTimeOffset StartDate, DateTimeOffset EndDate, int start, int length, string search, string OrderBy, string OrderDir, string CompanyKey)
        {
            try
            {
           
                var SearchString = (search != null) ? search : string.Empty;
                

                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
                var pStartDate = new SqlParameter("@StartDate", StartDate);
                var pEndDate = new SqlParameter("@EndDate", EndDate);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pUserID = new SqlParameter("@UserID", UserID);
                var pRecordStart = new SqlParameter("@RecordStart", start);
                var pRecordLength = new SqlParameter("@RecordLength", length);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pCompanyKey = new SqlParameter("@UniqueKey", CompanyKey);

                var result = await _context.Set<DeliveryOutput>().FromSqlRaw(" exec Pro_Report_Message_Delivery @CompanyID, @UserID, @StartDate, @EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                 pCompanyID, pUserID, pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pCompanyKey).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while seeding into database {0},{1},{2},{3}", ex.Message, ex.InnerException, ex.StackTrace, ex.Source);
                return null;
            }
        }

        public DataTablePaging GetResponseReportByGroup(DataTableAjaxPostModel dtapm, DateTimeOffset startDate, DateTimeOffset endDate, string messageType, int drillOpt, int groupId, int objectMappingId, string companyKey, bool isThisWeek, bool isThisMonth, bool isLastMonth, int companyId)
        {
            DateTime stDate = DateTime.Now;
            DateTime enDate = DateTime.Now;

            GetStartEndDate(isThisWeek, isThisMonth, isLastMonth, ref stDate, ref enDate, startDate, endDate);

            var RecordStart = dtapm.Start == 0 ? 0 : dtapm.Start;
            var RecordLength = dtapm.Length == 0 ? int.MaxValue : dtapm.Length;
            var SearchString = (dtapm.Search != null) ? dtapm.Search.Value : "";
            string OrderBy = dtapm.Order != null ? dtapm.Order.FirstOrDefault().Column : "DateSent";
            string OrderDir = dtapm.Order != null ? dtapm.Order.FirstOrDefault().Dir : "desc";

            var returnData = GetPingReport(companyId, stDate, enDate, messageType, drillOpt, groupId, objectMappingId,
                RecordStart, RecordLength, SearchString, OrderBy, OrderDir, companyKey);

            int totalRecord = 0;

            if (returnData != null)
                totalRecord = returnData.Count;

            List<PingReportGrid> ttodata = GetPingReport(companyId, stDate, enDate, messageType, drillOpt, groupId, objectMappingId,
               0, int.MaxValue, "", "MessageListId", OrderDir, companyKey);

            if (ttodata != null)
                totalRecord = ttodata.Count;

            DataTablePaging rtn = new DataTablePaging();
            rtn.Draw = dtapm.Draw;
            rtn.RecordsTotal = totalRecord;
            rtn.RecordsFiltered = returnData.Count;
            rtn.Data = returnData;
            return rtn;
        }

        public List<IncidentMessageAuditResponse> GetIndidentMessagesAudit(int incidentActivationId, int companyId)
        {
            var pIncidentActivationId = new SqlParameter("@IncidentActivationId", incidentActivationId);
            var pCompanyId = new SqlParameter("@CompanyID", companyId);
            var incidentMessageDetails = _context.Set<IncidentMessageAuditResponse>().FromSqlRaw(
                "Pro_Report_GetIncidentMessagesAudit @IncidentActivationID, @CompanyID",
                pIncidentActivationId,
                pCompanyId).ToList();
            return incidentMessageDetails;
        }

        public List<IncidentUserLocationResponse> GetIncidentUserLocation(int incidentActivationId, int companyId)
        {
            var pIncidentActivationId = new SqlParameter("@IncidentActivationID", incidentActivationId);
            var pCompanyId = new SqlParameter("@CompanyID", companyId);
            var uInfoList = _context.Set<IncidentUserLocationResponse>().FromSqlRaw(
                "Pro_Report_GetIncidentUserLocation @IncidentActivationID, @CompanyID",
                pIncidentActivationId,
                pCompanyId).ToList();
            return uInfoList;
        }

        public List<TrackUsers> GetTrackingUsers(string status, int userId, int companyId)
        {
            var pCompanyID = new SqlParameter("@CompanyID", companyId);
            var pUserID = new SqlParameter("@UserID", userId);
            var pStatus = new SqlParameter("@Status", status);

            var tckusr = _context.Set<TrackUsers>().FromSqlRaw("Pro_Get_Tracking_Users @CompanyID, @UserID, @Status",
                pCompanyID, pUserID, pStatus).ToList();

            return tckusr;
        }


        private DateTime GetLocalTime(string timeZoneId, DateTime? paramTime = null)
        {
            try
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
            catch (Exception ex) { }
            return DateTime.Now;
        }

        private List<PingReportGrid> GetPingReport(int companyId, DateTime startDate, DateTime endDate, string messageType, int drillOpt, int groupId, int objectMappingId,
                        int recordStart, int recordLength, string searchString, string orderBy, string orderDir, string companyKey)
        {
            var pStartDate = new SqlParameter("@StartDate", startDate);
            var pEndDate = new SqlParameter("@EndDate", endDate);
            var pCompanyID = new SqlParameter("@CompanyID", companyId);
            var pMessageType = new SqlParameter("@MessageType", messageType);
            var pDrillOption = new SqlParameter("@DrillOption", drillOpt);
            var pGroupID = new SqlParameter("@GroupID", groupId);
            var pObjectMappingID = new SqlParameter("@ObjectMappingID", objectMappingId);
            var pRecordStart = new SqlParameter("@RecordStart", recordStart);
            var pRecordLength = new SqlParameter("@RecordLength", recordLength);
            var pSearchString = new SqlParameter("@SearchString", searchString);
            var pOrderBy = new SqlParameter("@OrderBy", orderBy);
            var pOrderDir = new SqlParameter("@OrderDir", orderDir);
            var pUniqueKey = new SqlParameter("@UniqueKey", companyKey);

            var result = new List<PingReportGrid>();
            var propertyInfo = typeof(PingReportGrid).GetProperty(orderBy);

            if (orderDir == "desc")
            {
                result = _context.Set<PingReportGrid>().FromSqlRaw("Pro_Report_Ping @StartDate, @EndDate, @CompanyID, @MessageType, @DrillOption, @GroupID, @ObjectMappingID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                    pStartDate, pEndDate, pCompanyID, pMessageType, pDrillOption, pGroupID, pObjectMappingID, pRecordStart, pRecordLength,
                    pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                    .ToList().Select(c => {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    })
                    .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
            }
            else
            {
                result = _context.Set<PingReportGrid>().FromSqlRaw("Pro_Report_Ping @StartDate, @EndDate, @CompanyID, @MessageType, @DrillOption, @GroupID, @ObjectMappingID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                    pStartDate, pEndDate, pCompanyID, pMessageType, pDrillOption, pGroupID, pObjectMappingID, pRecordStart, pRecordLength,
                    pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                    .ToList().Select(c => {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    })
                    .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
            }
            return result;
        }

        private async Task<string> LookupWithKey(string Key, string Default = "")
        {
            try
            {
                Dictionary<string, string> Globals = CCConstants.GlobalVars;
                if (Globals.ContainsKey(Key))
                {
                    return Globals[Key];
                }


                var LKP = await _context.Set<SysParameter>().Where(w => w.Name == Key).FirstOrDefaultAsync();
                if (LKP != null)
                {
                    Default = LKP.Value;
                }
                return Default;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                        ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return Default;
            }
        }

        private void GetStartEndDate(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, ref DateTime stDate, ref DateTime enDate, DateTimeOffset StartDate, DateTimeOffset EndDate)
        {
            if (IsThisWeek)
            {
                int dayofweek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                stDate = DateTime.Now.AddDays(0 - dayofweek);
                enDate = DateTime.Now.AddDays(7 - dayofweek);
                stDate = new DateTime(stDate.Year, stDate.Month, stDate.Day, 0, 0, 0);
                enDate = new DateTime(enDate.Year, enDate.Month, enDate.Day, 23, 59, 59);
            }
            else if (IsThisMonth)
            {
                stDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                enDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23, 59, 59);
            }
            else if (IsLastMonth)
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
                stDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
                enDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);
            }
        }

    }
}
