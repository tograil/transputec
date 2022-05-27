using CrisesControl.Core.Compatibility;
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
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories {
    public class ReportRepository : IReportsRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<ReportRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
      

        private int UserID;
        private int CompanyID;
        private string UniqueKey;

        public ReportRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor, ILogger<ReportRepository> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            this._logger = logger;
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

        public async Task<List<MessageAcknowledgements>> GetIndidentMessageAck(int MessageId, int MessageAckStatus, int MessageSentStatus, int RecordStart, int RecordLength, string SearchString, string OrderBy, string OrderDir, int CurrentUserId, string Filters, string CompanyKey, string Source)
        {
            try
            {
                var pMessageId = new SqlParameter("@MessageID", MessageId);
                var pMessageAckStatus = new SqlParameter("@MessageAckStatus", MessageAckStatus);
                var pMessageSentStatus = new SqlParameter("@MessageSentStatus", MessageSentStatus);
                var pUserID = new SqlParameter("@UserID", CurrentUserId);
                var pSource = new SqlParameter("@Source", Source);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pFilters = new SqlParameter("@Filters", Filters);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

                var ack_list = await  _context.Set<MessageAcknowledgements>().FromSqlRaw("exec Pro_Get_Message_Acknowledgements @MessageID, @MessageAckStatus, @MessageSentStatus, @UserID,@Source, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@Filters,@UniqueKey",
                    pMessageId, pMessageAckStatus, pMessageSentStatus, pUserID, pSource, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pFilters, pUniqueKey).ToListAsync();
                return ack_list;
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

               List<DataTablePaging> rtn = new List<DataTablePaging>(await _context.Set<DataTablePaging>().Where(x=>x.draw== draw).Select(n=>
                new DataTablePaging() { 
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = result.Count,
                data = result
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

        public GetCurrentIncidentStatsResponse GetCurrentIncidentStats(string timeZoneId)
        {
            DateTimeOffset baseDate = GetLocalTime(timeZoneId, DateTime.Now.Date);
            var today = baseDate;
            var startDate = GetLocalTime(timeZoneId, DateTime.Now.Date.AddDays(1 - DateTime.Now.Day));

            var pStartDate = new SqlParameter("@StartDate", startDate);
            var pEndDate = new SqlParameter("@EndDate", today);
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var result = _context.Set<GetCurrentIncidentStatsResponse>().FromSqlRaw("Pro_Report_GetCurrentIncidentStats @StartDate, @EndDate, @CompanyID",
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
                incidentReportData.KeyContacts = _context.Set<GetIncidentDataByActivationRefKeyContactsResponse>().FromSqlRaw(
                    "Pro_Report_GetIncidentData_ByActivationRef_KeyContacts @IncidentActivationID, @CompanyID",
                    pIncidentActivationID2,
                    pCompanyID2).ToList();
                var pIncidentActivationID3 = new SqlParameter("@IncidentActivationID", incidentActivationId);
                var pCompanyID3 = new SqlParameter("@CompanyID", companyId);
                incidentReportData.IncidentAssets = _context.Set<GetIncidentDataByActivationRefIncidentAssetsResponse>().FromSqlRaw(
                    "Pro_Report_GetIncidentData_ByActivationRef_IncidentAssets @IncidentActivationID, @CompanyID",
                    pIncidentActivationID3,
                    pCompanyID3).ToList();
                return incidentReportData;
            }
            else
            {
                return incidentReportData;
            }
        }

        public DateTime GetLocalTime(string timeZoneId, DateTime? paramTime = null)
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

        public List<PingReportGrid> GetPingReport(int companyId, DateTime startDate, DateTime endDate, string messageType, int drillOpt, int groupId, int objectMappingId,
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
    }
}
