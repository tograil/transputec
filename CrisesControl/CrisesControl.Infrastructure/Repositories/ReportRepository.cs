using CrisesControl.Core.Companies;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Models;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
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
        public async Task<string> GetTimeZoneVal(int UserId)
        {
            return (await _context.Set<User>()
                .Include(x=>x.Company)
               // .Include(x => x.StdTimeZone)
                .FirstOrDefaultAsync(x => x.UserId == UserId))?.Company.StdTimeZone?.ZoneLabel ?? "GMT Standard Time";
        }

        public async Task<dynamic> GetMessageDeliverySummary(int MessageID)
        {
            try
            {

                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                var pMessageID = new SqlParameter("@MessageID", MessageID);

                  
                    var result = await _context.Set<DeliverySummary>().FromSqlRaw(" exec Pro_Report_Delivery_Summary @MessageID", pMessageID).ToListAsync();

                    List<DeliverySummary> DlvRecs = new List<DeliverySummary>();
                    string TimeZoneId = await GetTimeZoneVal(UserID);
                    List<DateTimeOffset> endTimes = new List<DateTimeOffset>();

                    foreach (DeliverySummary rec in result)
                    {
                        endTimes.Add(rec.EmailEndTime);
                        endTimes.Add(rec.PhoneEndTime);
                        endTimes.Add(rec.PushEndTime);
                        endTimes.Add(rec.TextEndTime);

                        rec.EmailStartTime = rec.EmailStartTime;
                        rec.EmailEndTime = rec.EmailEndTime;
                        rec.PhoneStartTime = rec.PhoneStartTime;
                        rec.PhoneEndTime = rec.PhoneEndTime;
                        rec.PushStartTime = rec.PushStartTime;
                        rec.PushEndTime = rec.PushEndTime;
                        rec.TextStartTime = rec.TextStartTime;
                        rec.TextEndTime = rec.TextEndTime;

                        DlvRecs.Add(rec);
                    }

                    var methods = await _context.Set<Message>().Where(M=>M.MessageId == MessageID).Select( M=> new  { M.Phone, M.Text, M.Email, M.Push, M.CreatedOn }).FirstOrDefaultAsync();
                 

                    endTimes.Add(methods.CreatedOn);

                    DateTimeOffset maxEndTimes = endTimes.Max();

                    return Tuple.Create(DlvRecs, methods, maxEndTimes);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);

                return null;
            }
        }
    }
}
