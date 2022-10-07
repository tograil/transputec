using AutoMapper;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Exceptions.NotFound;
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
using System.IO;
using System.Data;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using IncidentMessagesRtn = CrisesControl.Core.Reports.IncidentMessagesRtn;
using FailedTaskList = CrisesControl.Core.Reports.FailedTaskList;
using CrisesControl.Core.Import;
using CrisesControl.Api.Application.Helpers;

namespace CrisesControl.Infrastructure.Repositories {
    public class ReportsRepository : IReportsRepository {
        private readonly CrisesControlContext _context;
        private readonly ILogger<ReportsRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly DBCommon _DBC;
        // private readonly IQueueService _queueService;

        private int currentUserId;
        private int currentCompanyId;
        public DateTimeOffset MinTrackingDate = DateTimeOffset.Now;
        public DateTimeOffset MaxTrackingDate = DateTimeOffset.Now;
        public DateTimeOffset NewMaxTrackingDate = DateTimeOffset.Now;
        private IConfiguration configuration;

        public ReportsRepository(CrisesControlContext context,
                                IHttpContextAccessor httpContextAccessor,
                                ILogger<ReportsRepository> logger,
                                IMapper mapper, DBCommon DBC) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
            _DBC = DBC;
            //_queueService = queueService;

            currentUserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            currentCompanyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        }
        public async Task<List<SOSItem>> GetSOSItems(int UserId) {
            try {

                var pUserId = new SqlParameter("@UserID", UserId);
                var pCompanyID = new SqlParameter("@CompanyID", currentCompanyId);

                var result = await _context.Set<SOSItem>().FromSqlRaw("exec Pro_Get_SOS_Alerts {0},{1}", pCompanyID, pUserId).ToListAsync();

                return result;

            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<List<IncidentPingStatsCount>> GetIncidentPingStats(int companyID, int noOfMonth) {
            try {

                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                var pNoOfMonth = new SqlParameter("@NoOfMonth", noOfMonth);

                var result = await _context.Set<IncidentPingStatsCount>().FromSqlRaw("exec Pro_Report_Dashboard_Incident_Ping_Stats_ByMonth {0},{1}", pCompanyID, pNoOfMonth).ToListAsync();
                return result;


            } catch (Exception ex) {
            }
            return new List<IncidentPingStatsCount>();
        }

        public async Task<DataTablePaging> GetIncidentMessageAck(int messageId, int messageAckStatus, int messageSentStatus, string source, int recordStart, int recordLength,
            string searchString = "", string orderBy = "DateAcknowledge", string orderDir = "asc", string filters = "", string uniqueKey = "") {

            orderBy = orderBy ?? "DateAcknowledge";
            orderDir = orderDir ?? "asc";
            source = source ?? "WEB";

            var propertyInfo = typeof(MessageAcknowledgements).GetProperty(orderBy);
            var incidentMessageListDetails = new List<MessageAcknowledgements>();
            int totalRecord = 0;

            string ig = _DBC.LookupWithKey("INITIALS_GENERATOR_URL");

            if (orderDir == "desc") {
                incidentMessageListDetails = GetAcknowledgements(messageId, messageAckStatus, messageSentStatus, recordStart, recordLength, searchString,
                orderBy, orderDir, filters, uniqueKey, source).Result.Select(c => {
                    c.ActiveUser = c.UserStatus == 1 ? true : false;
                    c.AcknowledgedUser = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    c.UserMobile = new PhoneNumber { ISD = c.MobileNo != null ? c.ISDCode : "", Number = c.MobileNo != null ? c.MobileNo : "" };
                    c.UserLandLine = new PhoneNumber { ISD = c.LLISDCode != null ? c.LLISDCode : "", Number = c.Landline != null ? c.Landline : "" };
                    //if(string.IsNullOrEmpty(c.UserPhoto)) {
                    c.UserPhoto = ig + "/" + c.UserId.ToString() + "/" + currentCompanyId.ToString() + "/" + c.FirstName + " " + c.LastName;
                    //}
                    return c;
                }).OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();

            } else {
                incidentMessageListDetails = GetAcknowledgements(messageId, messageAckStatus, messageSentStatus, recordStart, recordLength, searchString,
                orderBy, orderDir, filters, uniqueKey, source).Result.Select(c => {
                    c.ActiveUser = c.UserStatus == 1 ? true : false;
                    c.AcknowledgedUser = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    c.UserMobile = new PhoneNumber { ISD = c.MobileNo != null ? c.ISDCode : "", Number = c.MobileNo != null ? c.MobileNo : "" };
                    c.UserLandLine = new PhoneNumber { ISD = c.LLISDCode != null ? c.LLISDCode : "", Number = c.Landline != null ? c.Landline : "" };
                    //if(string.IsNullOrEmpty(c.UserPhoto)) {
                    c.UserPhoto = ig + "/" + c.UserId.ToString() + "/" + currentCompanyId.ToString() + "/" + c.FirstName + " " + c.LastName;
                    //}
                    return c;
                }).OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
            }

            if (messageAckStatus == 0 || messageAckStatus == 1) {
                totalRecord = GetAcknowledgements(messageId, messageAckStatus, messageSentStatus, 0, int.MaxValue, "",
                "U.UserId", "asc", "", uniqueKey, source).Result.Count();
            } else {
                totalRecord = GetAcknowledgements(messageId, 0, messageSentStatus, 0, int.MaxValue, "",
               "U.UserId", "asc", "", uniqueKey, source).Result.Count();
            }

            DataTablePaging rtn = new DataTablePaging();
            rtn.RecordsTotal = totalRecord;
            rtn.RecordsFiltered = incidentMessageListDetails.Count;
            rtn.Data = incidentMessageListDetails;

            return rtn;
        }

        public async Task<List<MessageAcknowledgements>> GetAcknowledgements(int MessageId, int MessageAckStatus, int MessageSentStatus, int RecordStart, int RecordLength, string search,
             string OrderBy, string OrderDir, string Filters, string CompanyKey, string Source = "WEB") {
            try {
                const string ord = "PrimaryEmail";
                const string dir = "asc";
                
                var SearchString = (search != null) ? search : string.Empty;

                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = ord;

                if (string.IsNullOrEmpty(OrderDir))
                    OrderDir = dir;

                var pMessageId = new SqlParameter("@MessageID", MessageId);
                var pMessageAckStatus = new SqlParameter("@MessageAckStatus", MessageAckStatus);
                var pMessageSentStatus = new SqlParameter("@MessageSentStatus", MessageSentStatus);
                var pUserID = new SqlParameter("@UserID", currentUserId);
                var pSource = new SqlParameter("@Source", Source);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pFilters = new SqlParameter("@Filters", Filters);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

                var ackList = await _context.Set<MessageAcknowledgements>().FromSqlRaw("exec Pro_Get_Message_Acknowledgements @MessageID, @MessageAckStatus, @MessageSentStatus, @UserID,@Source, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@Filters,@UniqueKey",
                    pMessageId, pMessageAckStatus, pMessageSentStatus, pUserID, pSource, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pFilters, pUniqueKey).ToListAsync();


                return ackList;
            } catch (Exception ex) {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return null;
            }
        }

        public async Task<List<ResponseSummary>> ResponseSummary(int MessageID) {
            try {
                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var result = await _context.Set<ResponseSummary>().FromSqlRaw("exec Pro_Get_Message_Acknowledgements_Responses @MessageID", pMessageID).ToListAsync();

                return result;
            } catch (Exception ex) {

                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return new List<ResponseSummary>();
        }

        public async Task<List<DataTablePaging>> GetIncidentMessageNoAck(int draw, int IncidentActivationId, int RecordStart, int RecordLength, string? SearchString, string? UniqueKey) {
            try {
                const string OrderBy = "UserId";
                const string OrderDir = "asc";
                
                var pIncidentActivationId = new SqlParameter("@ActiveIncidentID", IncidentActivationId);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pUserID = new SqlParameter("@UserID", currentUserId);
                var pCompanyID = new SqlParameter("@CompanyID", currentCompanyId);
                var pUniqueKey = new SqlParameter("@UniqueKey", UniqueKey);

                var result = await _context.Set<MessageAcknowledgements>().FromSqlRaw("exec Pro_Get_No_Ack_Users @ActiveIncidentID, @UserID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                   pIncidentActivationId, pUserID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey).ToListAsync();

                int totalRecord = 0;
                totalRecord = result.Count;

                List<DataTablePaging> rtn = new List<DataTablePaging>(await _context.Set<DataTablePaging>().Where(x => x.Draw == draw).Select(n =>
                 new DataTablePaging() {
                     Draw = draw,
                     RecordsTotal = totalRecord,
                     RecordsFiltered = result.Count,
                     Data = result
                 }).ToListAsync());
                return rtn;
            } catch (Exception ex) {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                          ex.Message, ex.StackTrace, ex.InnerException, ex.Source);

            }
            return new List<DataTablePaging>();


        }

        public CurrentIncidentStatsResponse GetCurrentIncidentStats(string timeZoneId, int companyId) {
            DateTimeOffset baseDate = GetLocalTime(timeZoneId, DateTime.Now.Date);
            var today = baseDate;
            var startDate = GetLocalTime(timeZoneId, DateTime.Now.Date.AddDays(1 - DateTime.Now.Day));

            var pStartDate = new SqlParameter("@StartDate", startDate);
            var pEndDate = new SqlParameter("@EndDate", today);
            var pCompanyID = new SqlParameter("@CompanyID", companyId);
            var result = _context.Set<CurrentIncidentStatsResponse>().FromSqlRaw("Pro_Report_GetCurrentIncidentStats @StartDate, @EndDate, @CompanyID",
                pStartDate,
                pEndDate,
                pCompanyID).ToList()?.FirstOrDefault();
            return result;
        }

        public IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId) {
            var pIncidentActivationID = new SqlParameter("@IncidentActivationID", incidentActivationId);
            var pCompanyID = new SqlParameter("@CompanyID", companyId);
            var pUserID = new SqlParameter("@UserID", userId);
            var incidentReportData = _context.Set<IncidentData>().FromSqlRaw(
                "Pro_Report_GetIncidentData_ByActivationRef @IncidentActivationID, @CompanyID, @UserID",
                pIncidentActivationID,
                pCompanyID, pUserID).ToList()?.FirstOrDefault();
            if (incidentReportData != null) {
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

        public async Task<List<PingGroupChartCount>> GetPingReportChart(DateTime StartDate, DateTime EndDate, int GroupID, string MessageType, int ObjectMappingID) {
            try {

                var pStartDate = new SqlParameter("@StartDate", StartDate);
                var pEndDate = new SqlParameter("@EndDate", EndDate);
                var pGroupID = new SqlParameter("@GroupID", GroupID);
                var pMessageType = new SqlParameter("@MessageType", MessageType);
                var pObjectMappingID = new SqlParameter("@ObjectMappingID", ObjectMappingID);
                var pCompanyID = new SqlParameter("@CompanyID", currentCompanyId);
                var pUserID = new SqlParameter("@UserID", currentUserId);

                var result = await _context.Set<PingGroupChartCount>().FromSqlRaw("exec Pro_Report_Ping_Chart @StartDate, @EndDate, @GroupID, @MessageType, @ObjectMappingID, @CompanyID,@UserID",
                pStartDate, pEndDate, pGroupID, pMessageType, pObjectMappingID, pCompanyID, pUserID).ToListAsync();



                return result;

            } catch (Exception ex) {
                _logger.LogError("Error occure while trying to seeding into the database {0},{1},{2}", ex.Message, ex.InnerException, ex.StackTrace);

            }
            return new List<PingGroupChartCount>();
        }

        public async Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "") {
            try {
                Key = Key.ToUpper();

                if (CompanyId > 0) {
                    var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
                    if (LKP != null) {
                        Default = LKP.Value;
                    } else {

                        var LPR = await _context.Set<LibCompanyParameter>().Where(CP => CP.Name == Key).FirstOrDefaultAsync();
                        if (LPR != null) {
                            Default = LPR.Value;
                        } else {
                            Default = await LookupWithKey(Key, Default);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(CustomerId) || !string.IsNullOrEmpty(Key)) {

                    var cmp = await _context.Set<Company>().Where(w => w.CustomerId == CustomerId).FirstOrDefaultAsync();
                    if (cmp != null) {
                        var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
                        if (LKP != null) {
                            Default = LKP.Value;
                        }
                    } else {
                        Default = "NOT_EXIST";
                    }
                }

                return Default;
            } catch (Exception ex) {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                      ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return Default;
            }
        }

        public async Task<List<DeliveryOutput>> GetMessageDeliveryReport(DateTimeOffset StartDate, DateTimeOffset EndDate, int start, int length, string search, string OrderBy, string OrderDir, string CompanyKey) {
            try {

                var SearchString = (search != null) ? search : string.Empty;

                var pStartDate = new SqlParameter("@StartDate", StartDate);
                var pEndDate = new SqlParameter("@EndDate", EndDate);
                var pCompanyID = new SqlParameter("@CompanyID", currentCompanyId);
                var pUserID = new SqlParameter("@UserID", currentUserId);
                var pRecordStart = new SqlParameter("@RecordStart", start);
                var pRecordLength = new SqlParameter("@RecordLength", length);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pCompanyKey = new SqlParameter("@UniqueKey", CompanyKey);

                var result = await _context.Set<DeliveryOutput>().FromSqlRaw(" exec Pro_Report_Message_Delivery @CompanyID, @UserID, @StartDate, @EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                 pCompanyID, pUserID, pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pCompanyKey).ToListAsync();

                return result;
            } catch (Exception ex) {
                _logger.LogError("Error occurred while seeding into database {0},{1},{2},{3}", ex.Message, ex.InnerException, ex.StackTrace, ex.Source);
                return null;
            }

        }
        public async Task<string> GetTimeZoneVal(int UserId) {
            return (await _context.Set<User>()
                .Include(x => x.Company)
                // .Include(x => x.StdTimeZone)
                .FirstOrDefaultAsync(x => x.UserId == UserId))?.Company.StdTimeZone?.ZoneLabel ?? "GMT Standard Time";
        }

        public async Task<dynamic> GetMessageDeliverySummary(int MessageID) {
            try {

                var pMessageID = new SqlParameter("@MessageID", MessageID);


                var result = await _context.Set<DeliverySummary>().FromSqlRaw(" exec Pro_Report_Delivery_Summary @MessageID", pMessageID).ToListAsync();

                List<DeliverySummary> DlvRecs = new List<DeliverySummary>();
                string TimeZoneId = await GetTimeZoneVal(currentUserId);
                List<DateTimeOffset> endTimes = new List<DateTimeOffset>();

                foreach (DeliverySummary rec in result) {
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

                var methods = await _context.Set<Message>().Where(M => M.MessageId == MessageID).Select(M => new { M.Phone, M.Text, M.Email, M.Push, M.CreatedOn }).FirstOrDefaultAsync();


                endTimes.Add(methods.CreatedOn);

                DateTimeOffset maxEndTimes = endTimes.Max();

                return Tuple.Create(DlvRecs, methods, maxEndTimes);

            } catch (Exception ex) {
                throw new MessageNotFoundException(currentCompanyId, currentUserId);

                return null;
            }
        }

        public DataTablePaging GetResponseReportByGroup(DataTableAjaxPostModel dtapm, DateTimeOffset startDate, DateTimeOffset endDate, string messageType, int drillOpt, int groupId, int objectMappingId, string companyKey, bool isThisWeek, bool isThisMonth, bool isLastMonth, int companyId) {
            DateTime stDate = DateTime.Now;
            DateTime enDate = DateTime.Now;

            GetStartEndDate(isThisWeek, isThisMonth, isLastMonth, ref stDate, ref enDate, startDate, endDate);

            var RecordStart = dtapm.Start == 0 ? 0 : dtapm.Start;
            var RecordLength = dtapm.Length == 0 ? int.MaxValue : dtapm.Length;
            var SearchString = (dtapm.Search != null) ? dtapm.Search.Value : "";
            string OrderBy = dtapm.Order != null ? dtapm.Order : "DateSent";
            string OrderDir = dtapm.Dir != null ? dtapm.Dir : "desc";

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

        public List<IncidentMessageAuditResponse> GetIncidentMessagesAudit(int incidentActivationId, int companyId) {
            var pIncidentActivationId = new SqlParameter("@IncidentActivationId", incidentActivationId);
            var pCompanyId = new SqlParameter("@CompanyID", companyId);
            var incidentMessageDetails = _context.Set<IncidentMessageAuditResponse>().FromSqlRaw(
                "Pro_Report_GetIncidentMessagesAudit @IncidentActivationID, @CompanyID",
                pIncidentActivationId,
                pCompanyId).ToList();
            return incidentMessageDetails;
        }

        public List<IncidentUserLocationResponse> GetIncidentUserLocation(int incidentActivationId, int companyId) {
            var pIncidentActivationId = new SqlParameter("@IncidentActivationID", incidentActivationId);
            var pCompanyId = new SqlParameter("@CompanyID", companyId);
            var uInfoList = _context.Set<IncidentUserLocationResponse>().FromSqlRaw(
                "Pro_Report_GetIncidentUserLocation @IncidentActivationID, @CompanyID",
                pIncidentActivationId,
                pCompanyId).ToList();
            return uInfoList;
        }

        public List<TrackUsers> GetTrackingUsers(string status, int userId, int companyId) {
            var pCompanyID = new SqlParameter("@CompanyID", companyId);
            var pUserID = new SqlParameter("@UserID", userId);
            var pStatus = new SqlParameter("@Status", status);

            var tckusr = _context.Set<TrackUsers>().FromSqlRaw("Pro_Get_Tracking_Users @CompanyID, @UserID, @Status",
                pCompanyID, pUserID, pStatus).ToList();

            return tckusr;
        }


        private DateTime GetLocalTime(string timeZoneId, DateTime? paramTime = null) {
            try {
                if (string.IsNullOrEmpty(timeZoneId))
                    timeZoneId = "GMT Standard Time";

                DateTime retDate = DateTime.Now.ToUniversalTime();

                DateTime dateTimeToConvert = new DateTime(retDate.Ticks, DateTimeKind.Unspecified);

                DateTime timeUtc = DateTime.UtcNow;

                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                retDate = TimeZoneInfo.ConvertTimeFromUtc(dateTimeToConvert, cstZone);

                return retDate;
            } catch (Exception ex) { }
            return DateTime.Now;
        }

        private List<PingReportGrid> GetPingReport(int companyId, DateTime startDate, DateTime endDate, string messageType, int drillOpt, int groupId, int objectMappingId,
                        int recordStart, int recordLength, string searchString, string orderBy, string orderDir, string companyKey) {
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

            if (orderDir == "desc") {
                result = _context.Set<PingReportGrid>().FromSqlRaw("exec Pro_Report_Ping @StartDate, @EndDate, @CompanyID, @MessageType, @DrillOption, @GroupID, @ObjectMappingID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                    pStartDate, pEndDate, pCompanyID, pMessageType, pDrillOption, pGroupID, pObjectMappingID, pRecordStart, pRecordLength,
                    pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                    .ToList().Select(c => {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    })
                    .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
            } else {
                result = _context.Set<PingReportGrid>().FromSqlRaw("exec Pro_Report_Ping @StartDate, @EndDate, @CompanyID, @MessageType, @DrillOption, @GroupID, @ObjectMappingID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
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

        private async Task<string> LookupWithKey(string Key, string Default = "") {
            try {
                Dictionary<string, string> Globals = CCConstants.GlobalVars;
                if (Globals.ContainsKey(Key)) {
                    return Globals[Key];
                }


                var LKP = await _context.Set<SysParameter>().Where(w => w.Name == Key).FirstOrDefaultAsync();
                if (LKP != null) {
                    Default = LKP.Value;
                }
                return Default;
            } catch (Exception ex) {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                        ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return Default;
            }
        }

        public void GetStartEndDate(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, ref DateTime stDate, ref DateTime enDate, DateTimeOffset StartDate, DateTimeOffset EndDate) {
            if (IsThisWeek) {
                int dayofweek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                stDate = DateTime.Now.AddDays(0 - dayofweek);
                enDate = DateTime.Now.AddDays(7 - dayofweek);
                stDate = new DateTime(stDate.Year, stDate.Month, stDate.Day, 0, 0, 0);
                enDate = new DateTime(enDate.Year, enDate.Month, enDate.Day, 23, 59, 59);
            } else if (IsThisMonth) {
                stDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                enDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23, 59, 59);
            } else if (IsLastMonth) {
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year;
                int month = currentDate.Month;

                if (month == 1) {
                    year = year - 1;
                    month = 12;
                } else {
                    month = month - 1;
                }
                stDate = new DateTime(year, month, 1, 0, 0, 0);
                enDate = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
            } else {
                stDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
                enDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);
            }
        }


        public async Task<List<TrackUserCount>> GetTrackingUserCount(int companyId) {
            try {


                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var tckusr = await _context.Set<TrackUserCount>().FromSqlRaw("exec Pro_Get_Tracking_Users_Count @CompanyID", pCompanyID).ToListAsync();

                return tckusr;

            } catch (Exception ex) {
                _logger.LogError("Error occured while seeding the database {0},{1}", ex.Message, ex.InnerException);

            }
            throw new CompanyNotFoundException(companyId, currentUserId);

        }
        public async Task<List<IncidentMessagesRtn>> GetIncidentReportDetails(int IncidentActivationID, int CompanyID, int UserId) {
            try {

                var pIncidentActivationID = new SqlParameter("@IncidentActivationID", IncidentActivationID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pUserID = new SqlParameter("@UserID", UserId);

                var incimsgs = await _context.Set<IncidentMessagesRtn>().FromSqlRaw("exec Pro_Get_Incident_Messages @IncidentActivationID, @CompanyID, @UserID",
                  pIncidentActivationID, pCompanyID, pUserID).ToListAsync();


                incimsgs.Select(async c => {
                    c.SentBy = new UserFullName { Firstname = c.SentByFirst, Lastname = c.SentByLast };
                    c.Notes = await _context.Set<IncidentTaskNote>()
                               .Where(N => N.ObjectId == IncidentActivationID && N.NoteType == "TASK"
                               || N.ObjectId == IncidentActivationID && N.NoteType == "INCIDENT" && c.MessageType == "Ping"
                               ).FirstOrDefaultAsync();
                    return c;
                }).ToList();



                return incimsgs;

            } catch (Exception ex) {
                throw new ReportingNotFoundException(CompanyID, UserId);
            }
        }
        public async Task<DataTablePaging> GetIncidentReport(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate, int SelectedUserID, int CompanyId) {
            try {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                GetStartEndDate(IsThisWeek, IsThisMonth, IsLastMonth, ref stDate, ref enDate, StartDate, EndDate);
                int MaxIncidentReport = 50;
                int.TryParse(await LookupWithKey("MAX_INCIDENT_REPORT_ITEMS"), out MaxIncidentReport);

                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
                var pUserID = new SqlParameter("@UserID", SelectedUserID);

                var incidentReportData = await _context.Set<IncidentReportResponse>().FromSqlRaw(
                    " exec Pro_Report_GetIncidentData @StartDate, @EndDate, @CompanyID, @UserID",
                    pStartDate, pEndDate, pCompanyID, pUserID).ToListAsync();
                foreach (var incident in incidentReportData) {
                    var pStartDate2 = new SqlParameter("@StartDate", stDate);
                    var pEndDate2 = new SqlParameter("@EndDate", enDate);
                    var pCompanyID2 = new SqlParameter("@CompanyID", CompanyId);
                    var pIncidentActivationID = new SqlParameter("@IncidentActivationId", incident.IncidentActivationId);
                    incident.KeyContacts = await _context.Set<IncidentReportKeyContactResponse>().FromSqlRaw(
                        "exec Pro_Report_GetIncidentData_KeyContacts @StartDate, @EndDate, @CompanyID, @IncidentActivationId",
                        pStartDate2,
                        pEndDate2,
                        pCompanyID2,
                        pIncidentActivationID).ToListAsync();
                }


                int TotalRecord = incidentReportData.Count;
                var Report = incidentReportData.Take(MaxIncidentReport).ToList();

                DataTablePaging rtn = new DataTablePaging();
                rtn.RecordsTotal = TotalRecord;
                rtn.RecordsFiltered = Report.Count();
                rtn.Data = Report;
                return rtn;


                //else
                //{
                //    ResultDTO.ErrorId = 110;
                //    ResultDTO.Message = "No record found.";
                //}
                //return ResultDTO;
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<UserPieChart>> GetUserReportPiechartData(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate, int SelectedUserID, int CompanyId) {
            try {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                TimeSpan timespan = TimeSpan.FromMinutes(30);
                GetStartEndDate(IsThisWeek, IsThisMonth, IsLastMonth, ref stDate, ref enDate, StartDate, EndDate);

                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);
                var pUserID = new SqlParameter("@UserID", SelectedUserID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
                var userPieChartData = _context.Set<UserPieChart>().FromSqlRaw(
                    "exec Pro_Report_GetUserReportPieChartData @StartDate, @EndDate, @UserID, @CompanyID",
                    pStartDate,
                    pEndDate,
                    pUserID,
                    pCompanyID).ToList();

                if (userPieChartData != null) {
                    return userPieChartData;
                }
                return null;
            } catch (Exception ex) {
                throw ex;
            }
        }
        // REPORTS => User Report
        public async Task<List<UserIncidentReportResponse>> GetUserIncidentReport(DateTimeOffset startDate, DateTimeOffset endDate, bool isThisWeek, bool isThisMonth, bool isLastMonth, int SelectedUserID) {
            try {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                TimeSpan timespan = TimeSpan.FromMinutes(30);
                GetStartEndDate(isThisWeek, isThisMonth, isLastMonth, ref stDate, ref enDate, startDate, endDate);

                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);
                var pUserID = new SqlParameter("@UserID", SelectedUserID);
                var userReportData = await _context.Set<UserIncidentReportResponse>().FromSqlRaw(
                    " exec Pro_Report_GetUserIncidentReport @StartDate, @EndDate, @UserID",
                    pStartDate,
                    pEndDate,
                    pUserID).ToListAsync();


                return userReportData;

            } catch (Exception ex) {
                throw ex;
            }
        }


        public async Task<List<PingReport>> GetPingReportAnalysis(int MessageID, string MessageType, int _CompanyID) {
            try {


                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var pMessageType = new SqlParameter("@MessageType", MessageType);
                var pCompanyID = new SqlParameter("@CompanyID", _CompanyID);

                var result = await _context.Set<PingReport>().FromSqlRaw("exec Pro_Report_Ping_Analysis @MessageID, @MessageType, @CompanyID",
                pMessageID, pMessageType, pCompanyID).ToListAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<IncidentUserMessageResponse>> GetIncidentUserMessage(int IncidentActivationId, int SelectedUserId, int CompanyID) {
            try {
                var pIncidentActivationID = new SqlParameter("@IncidentActivationID", IncidentActivationId);
                var pSelectedUserId = new SqlParameter("@UserID", SelectedUserId);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var incidentMessageListDetails = await _context.Set<IncidentUserMessageResponse>().FromSqlRaw(
                    "exec Pro_Report_GetIncidentUserMessage @IncidentActivationId,@UserID,@CompanyID",
                    pIncidentActivationID,
                    pSelectedUserId,
                    pCompanyID).ToListAsync();


                return incidentMessageListDetails;


            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<IncidentStatsResponse> GetIncidentStats(int IncidentActivationId, int CompanyId) {
            try {
                var pIncidentActivationId = new SqlParameter("@IncidentActivationId", IncidentActivationId);
                var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
                var incidentReportData = await _context.Set<IncidentStatsResponse>().FromSqlRaw(
                    "exec Pro_Report_GetIncidentStats @IncidentActivationId,@CompanyID",
                    pIncidentActivationId,
                    pCompanyId).FirstOrDefaultAsync();

                var incidentActivationId2 = new SqlParameter("@IncidentActivationId", IncidentActivationId);
                var companyId2 = new SqlParameter("@CompanyID", CompanyId);
                incidentReportData.IncidentKPI = await _context.Set<IncidentStat>().FromSqlRaw(
                    " exec Pro_Report_GetIncidentStats_IncidentKPI @IncidentActivationId,@CompanyID",
                    incidentActivationId2,
                    companyId2).FirstOrDefaultAsync();

                return incidentReportData;


            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<PerformanceReport>> GetPerformanceReport(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, bool ShowDeletedGroups, int CurrentUserId, int CompanyId, DateTimeOffset StartDate, DateTimeOffset EndDate, string MessageType) {
            try {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;

                GetStartEndDate(IsThisWeek, IsThisMonth, IsLastMonth, ref stDate, ref enDate, StartDate, EndDate);
                var KPIs = _context.Set<CompanyParameter>().Where(KPI => KPI.CompanyId == CompanyId).Select(KPI => new { KPI.Name, KPI.Value });

                Dictionary<string, string> KpiD = new Dictionary<string, string>();

                foreach (var kpi in KPIs) {
                    if (!KpiD.Where(s => s.Key == kpi.Name).Any())
                        KpiD.Add(kpi.Name, kpi.Value);
                }

                Array KPILIst = KpiD.ToArray();

                List<dynamic> dynamicList = new List<dynamic>();

                var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
                var pUserID = new SqlParameter("@UserID", CurrentUserId);
                var pShowDeleted = new SqlParameter("@ShowDeleted", ShowDeletedGroups);
                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);
                var pGroupType = new SqlParameter("@GroupType", MessageType);

                var PerfData = await _context.Set<PerformanceReport>().FromSqlRaw("exec Pro_Report_Performance @CompanyID, @UserID, @ShowDeleted, @StartDate, @EndDate, @GroupType",
                    pCompanyID, pUserID, pShowDeleted, pStartDate, pEndDate, pGroupType).ToListAsync();

                if (PerfData != null) {
                    dynamicList.Add(PerfData);
                }
                dynamicList.Add(KpiD);
                return PerfData;

            } catch (Exception ex) {
                throw ex;
            }

        }
        public async Task<DataTablePaging> GetPerformanceReportByGroup(int draw, bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, int CompanyId, int start, int length, string search, string orderby, string dir, int FilterUser, string MessageType, string GroupName, string GroupType, int DrillOpt, DateTimeOffset StartDate, DateTimeOffset EndDate, string CompanyKey = "") {
            try {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;

                GetStartEndDate(IsThisWeek, IsThisMonth, IsLastMonth, ref stDate, ref enDate, StartDate, EndDate);

                DataTablePaging rtn = new DataTablePaging();

                var RecordStart = start == 0 ? 0 : start;
                var RecordLength = length == 0 ? int.MaxValue : length;
                var SearchString = (search != null) ? search : string.Empty;
                string OrderBy = orderby != null ? orderby.FirstOrDefault().ToString() : "DateSent";
                string OrderDir = dir != null ? dir.FirstOrDefault().ToString() : "desc";

                int totalRecord = 0;

                if (FilterUser > 0) {
                    var returnData = await GetPerformanceReport(stDate, enDate, MessageType, CompanyId, GroupName, GroupType,
                        FilterUser, DrillOpt, RecordStart, RecordLength, SearchString, OrderBy, OrderDir, CompanyKey);

                    totalRecord = returnData.Count;

                    List<PingReportGrid> ttodata = await GetPerformanceReport(stDate, enDate, MessageType, CompanyId, GroupName,
                        GroupType, FilterUser, DrillOpt, 0, int.MaxValue, "", OrderBy, OrderDir, CompanyKey);
                    if (ttodata != null) {
                        totalRecord = ttodata.Count;
                    }

                    rtn.RecordsFiltered = returnData.Count;
                    rtn.Data = returnData;

                } else {
                    var returnData = await GetPerformanceReportByGroup(stDate, enDate, MessageType, CompanyId, GroupName, GroupType,
                        DrillOpt, RecordStart, RecordLength, SearchString, OrderBy, OrderDir, CompanyKey);

                    totalRecord = returnData.Count;

                    List<PingReportGrid> ttodata = await GetPerformanceReportByGroup(stDate, enDate, MessageType, CompanyId, GroupName, GroupType,
                        DrillOpt, 0, int.MaxValue, "", OrderBy, OrderDir, CompanyKey);
                    if (ttodata != null) {
                        totalRecord = ttodata.Count;
                    }
                    rtn.RecordsFiltered = returnData.Count;
                    rtn.Data = returnData;
                }

                rtn.Draw = draw;
                rtn.RecordsTotal = totalRecord;


                return rtn;

            } catch (Exception ex) {
                throw ex;
            }

        }
        public async Task<List<PingReportGrid>> GetPerformanceReportByGroup(DateTime StartDate, DateTime EndDate, string MessageType, int _CompanyID, string GroupName = "", string GroupType = "", int DrillOpt = 0,
            int RecordStart = 0, int RecordLength = 100, string SearchString = "", string OrderBy = "DateSent", string OrderDir = "desc", string CompanyKey = "") {
            try {


                var pStartDate = new SqlParameter("@StartDate", StartDate);
                var pEndDate = new SqlParameter("@EndDate", EndDate);
                var pCompanyID = new SqlParameter("@CompanyID", _CompanyID);
                var pMessageType = new SqlParameter("@MessageType", MessageType);
                var pDrillOption = new SqlParameter("@DrillOption", DrillOpt);
                var pGroupName = new SqlParameter("@GroupName", string.IsNullOrEmpty(GroupName) ? null : GroupName);
                var pGroupType = new SqlParameter("@GroupType", string.IsNullOrEmpty(GroupType) ? null : GroupType);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);


                var result = new List<PingReportGrid>();
                var propertyInfo = typeof(PingReportGrid).GetProperty(OrderBy);

                if (OrderDir == "desc") {
                    result = _context.Set<PingReportGrid>().FromSqlRaw("exec Pro_Report_Performance_Groups @StartDate, @EndDate, @CompanyID, @MessageType, @GroupName, @GroupType, @DrillOption, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                pStartDate, pEndDate, pCompanyID, pMessageType, pGroupName, pGroupType, pDrillOption, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToList().Select(c => {
                            c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                            return c;
                        })
                        .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
                } else {
                    result = _context.Set<PingReportGrid>().FromSqlRaw("exec Pro_Report_Performance_Groups @StartDate, @EndDate, @CompanyID, @MessageType, @GroupName, @GroupType, @DrillOption, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                pStartDate, pEndDate, pCompanyID, pMessageType, pGroupName, pGroupType, pDrillOption, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToList().Select(c => {
                            c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                            return c;
                        })
                        .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
                }

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<PingReportGrid>> GetPerformanceReport(DateTime StartDate, DateTime EndDate, string MessageType, int companyId, string GroupName = "", string GroupType = "", int FilterUser = 0,
                int DrillOpt = 0, int RecordStart = 0, int RecordLength = 100, string SearchString = "", string OrderBy = "DateSent", string OrderDir = "desc", string CompanyKey = "") {
            try {


                var pStartDate = new SqlParameter("@StartDate", StartDate);
                var pEndDate = new SqlParameter("@EndDate", EndDate);
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pMessageType = new SqlParameter("@MessageType", MessageType);
                var pDrillOption = new SqlParameter("@DrillOption", DrillOpt);
                var pGroupName = new SqlParameter("@GroupName", string.IsNullOrEmpty(GroupName) ? null : GroupName);
                var pGroupType = new SqlParameter("@GroupType", string.IsNullOrEmpty(GroupType) ? null : GroupType);
                var pFilterUser = new SqlParameter("@FilterUser", FilterUser);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

                var result = new List<PingReportGrid>();
                var propertyInfo = typeof(PingReportGrid).GetProperty(OrderBy);

                if (OrderDir == "desc") {
                    result = await _context.Set<PingReportGrid>().FromSqlRaw("exec Pro_Report_Performance_Drill @StartDate, @EndDate, @CompanyID, @MessageType, @GroupName, @GroupType, @FilterUser, @DrillOption, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                pStartDate, pEndDate, pCompanyID, pMessageType, pGroupName, pGroupType, pFilterUser, pDrillOption, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToListAsync();
                    result.Select(c => {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    })
                            .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
                } else {
                    result = await _context.Set<PingReportGrid>().FromSqlRaw("exec Pro_Report_Performance_Drill @StartDate, @EndDate, @CompanyID, @MessageType, @GroupName, @GroupType, @FilterUser, @DrillOption, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                pStartDate, pEndDate, pCompanyID, pMessageType, pGroupName, pGroupType, pFilterUser, pDrillOption, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToListAsync();
                    result.Select(c => {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    })
                    .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
                }

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<DataTablePaging> GetResponseCoordinates(int start, int length, int MessageId) {
            try {
                var RecordStart = start == 0 ? 0 : start;
                var RecordLength = length == 0 ? int.MaxValue : length;

                var pMessageId = new SqlParameter("@MessageID", MessageId);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);


                var response = await _context.Set<ResponseCordinates>().FromSqlRaw("exec Pro_Response_Coordinates @MessageID, @RecordStart, @RecordLength",
                    pMessageId, pRecordStart, pRecordLength).ToListAsync();
                response.Select(c => {
                    c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    c.UserMobile = new PhoneNumber { ISD = c.ISDCode, Number = c.MobileNo };
                    return c;
                }).ToList();


                int totalRecord = _context.Set<MessageList>().Where(s => s.MessageId == MessageId &&
                s.MessageAckStatus == 1 &&
                s.MessageSentStatus == 1 &&
                s.UserLocationLat != null && s.UserLocationLong != null && s.UserLocationLat != "0" && s.UserLocationLong != "0").Count();

                DataTablePaging rtn = new DataTablePaging();
                rtn.RecordsTotal = totalRecord;
                rtn.RecordsFiltered = response.Count();
                rtn.Data = response;

                return rtn;
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<TrackingExport>> GetTrackingData(int TrackMeID, int UserDeviceID, DateTimeOffset StartDate, DateTimeOffset EndDate) {
            try {
                var pTrackMeID = new SqlParameter("@TrackMeID", TrackMeID);
                var pUserDeviceID = new SqlParameter("@UserDeviceID", UserDeviceID);
                var pStartDate = new SqlParameter("@StartDate", StartDate.DateTime.GetDateTimeOffset());
                var pEndDate = new SqlParameter("@EndDate", StartDate.DateTime.GetDateTimeOffset());

                var tckusr = await _context.Set<TrackingExport>().FromSqlRaw("exec Pro_ExportTrackingData @TrackMeID, @UserDeviceID, @StartDate, @EndDate",
                    pTrackMeID, pUserDeviceID, pStartDate, pEndDate).ToListAsync();
                tckusr.Select(c => {
                    c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    return c;
                }).ToList();

                if (tckusr.Count > 0) {
                    MinTrackingDate = tckusr.Select(s => s.CreatedOn).OrderBy(o => o).FirstOrDefault();
                    MaxTrackingDate = tckusr.Select(s => s.CreatedOn).OrderByDescending(o => o).FirstOrDefault();
                    NewMaxTrackingDate = await _context.Set<UserLocation1>().Where(w => w.UserDeviceId == UserDeviceID).Select(s => s.CreatedOn).OrderByDescending(o => o).FirstOrDefaultAsync();
                }

                return tckusr;
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<TaskPerformance> GetTaskPerformance(int companyId, bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate) {
            try {

                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;

                GetStartEndDate(IsThisWeek, IsThisMonth, IsLastMonth, ref stDate, ref enDate, StartDate, EndDate);

                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);

                var result = await _context.Set<TaskPerformance>().FromSqlRaw("exec Pro_Report_TaskKPI @CompanyID, @StartDate, @EndDate", pCompanyID, pStartDate, pEndDate).FirstOrDefaultAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<FailedTaskReport> GetFailedTasks(string ReportType, int companyId, bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate) {
            try {

                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;

                GetStartEndDate(IsThisWeek, IsThisMonth, IsLastMonth, ref stDate, ref enDate, StartDate, EndDate);

                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pReportType = new SqlParameter("@ReportType", ReportType);
                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);

                var result = await _context.Set<FailedTaskReport>().FromSqlRaw("exec Pro_Report_TaskKPI_Failed @CompanyID, @ReportType, @StartDate, @EndDate",
                    pCompanyID, pReportType, pStartDate, pEndDate).FirstOrDefaultAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<List<FailedTaskList>> GetFailedTaskList(string ReportType, int RangeMin, int RangeMax, DateTime stDate, DateTime enDate, int RecordStart, int RecordLength,
            string SearchString, string OrderBy, string OrderDir, string CompanyKey, int _CompanyID) {
            try {
                OrderBy = string.IsNullOrEmpty(OrderBy) ? "Name" : OrderBy;

                var pCompanyID = new SqlParameter("@CompanyID", _CompanyID);
                var pReportType = new SqlParameter("@ReportType", ReportType);
                var pRangeMin = new SqlParameter("@RangeMin", RangeMin);
                var pRangeMax = new SqlParameter("@RangeMax", RangeMax);
                var pStartDate = new SqlParameter("@StartDate", stDate);
                var pEndDate = new SqlParameter("@EndDate", enDate);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);


                var result = new List<FailedTaskList>();
                var propertyInfo = typeof(FailedTaskList).GetProperty(OrderBy);

                if (OrderDir == "desc") {
                    result = await _context.Set<FailedTaskList>().FromSqlRaw(" exec Pro_Report_TaskKPI_Failed_List @CompanyID, @ReportType,@RangeMin,@RangeMax,@StartDate, @EndDate, @RecordStart, @RecordLength,@SearchString,@OrderBy,@OrderDir,@UniqueKey",
                    pCompanyID, pReportType, pRangeMin, pRangeMax, pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToListAsync();
                    result.Select(c => {
                        c.TaskOwner = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.TaskCompletedBy = new UserFullName { Firstname = c.CompleteByFirstName, Lastname = c.CompleteByLastName };
                        return c;
                    })
                    .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
                } else {
                    result = await _context.Set<FailedTaskList>().FromSqlRaw(" exec Pro_Report_TaskKPI_Failed_List @CompanyID, @ReportType,@RangeMin,@RangeMax,@StartDate, @EndDate, @RecordStart, @RecordLength,@SearchString,@OrderBy,@OrderDir,@UniqueKey",
                    pCompanyID, pReportType, pRangeMin, pRangeMax, pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToListAsync();
                    result.Select(c => {
                        c.TaskOwner = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.TaskCompletedBy = new UserFullName { Firstname = c.CompleteByFirstName, Lastname = c.CompleteByLastName };
                        return c;
                    })
                            .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
                }

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<FailedAttempts>> GetFailedAttempts(int MessageListID, string CommsMethod) {
            try {


                var pMessageListID = new SqlParameter("@MessageListID", MessageListID);
                var pCommsMethod = new SqlParameter("@CommsMethod", CommsMethod);

                var result = await _context.Set<FailedAttempts>().FromSqlRaw("exec Pro_Report_Failed_Attempts @MessageListID, @CommsMethod",
                pMessageListID, pCommsMethod).ToListAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<string> DownloadDeliveryReport(int CompanyID, int MessageID, int UserID) {
            try {

                string FileName = "Delivery_Report-" + MessageID + ".csv";

                string ResultFilePath = Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + CompanyID + "\\DataExport\\";
                string FilePath = ExportPath + FileName;

                if (!Directory.Exists(ExportPath)) {
                    Directory.CreateDirectory(ExportPath);
                    await DeleteOldFiles(ExportPath);
                }

                if (File.Exists(FilePath)) {
                    File.Delete(FilePath);
                }

                string headerRow = string.Empty;

                var ExportData = await GetMessageDeliveryDetails(CompanyID, MessageID, UserID);

                using (StreamWriter SW = new StreamWriter(FilePath, false)) {

                    headerRow = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                   "Recipient", "Address/Destination", "Country Code", "Channel", "Status", "Attempt", "Sent Date", "Error Code", "Error Message");

                    SW.WriteLine(headerRow);

                    foreach (var row in ExportData) {

                        string rowdata = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                        row.Recipient, row.DeviceAddress, row.ISDCode, row.MethodName, row.Status, row.Attempts, row.MessageStartTime, row.ErrorCode, row.ErrorMessage);
                        if (!string.IsNullOrEmpty(rowdata))
                            SW.WriteLine(rowdata);
                    }
                }
                return FileName;
            } catch (Exception) {

                throw;
            }
        }
        public async Task<List<DeliveryDetails>> GetMessageDeliveryDetails(int CompanyID, int MessageID, int UserID) {
            try {


                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pUserID = new SqlParameter("@UserID", UserID);

                var result = await _context.Set<DeliveryDetails>().FromSqlRaw("exec Pro_Report_Customer_Delivery_Details @CompanyID, @MessageID, @UserID", pCompanyID, pMessageID, pUserID)
                    .ToListAsync();
                result.Select(c => {
                    if (c.MethodName.ToUpper() == "EMAIL") {
                        c.DeviceAddress = c.Recipient;
                    }
                    return c;
                }).ToList();
                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public string Getconfig(string key, string DefaultVal = "") {
            try {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (value != null) {
                    return value;
                } else {
                    return DefaultVal;
                }
            } catch (Exception ex) {
                throw ex;

            }
        }
        private async Task DeleteOldFiles(string dirName) {
            string[] files = Directory.GetFiles(@dirName);

            foreach (string file in files) {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddDays(-1))
                    fi.Delete();
            }
        }
        public async Task<List<DeliveryOutput>> GetUndeliveredMessage(int MessageID, string CommsMethod, string CountryCode, string ReportType, int RecordStart, int RecordLength,
            string SearchString, string OrderBy, string OrderDir, string CompanyKey) {
            try {


                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var pCommsMethod = new SqlParameter("@CommsMethod", CommsMethod);
                var pCountryCode = new SqlParameter("@CountryCode", CountryCode);
                var pReportType = new SqlParameter("@ReportType", ReportType);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pCompanyKey = new SqlParameter("@UniqueKey", CompanyKey);

                var result = await _context.Set<DeliveryOutput>().FromSqlRaw("exec Pro_Report_Undlivered_Message @MessageID, @CommsMethod, @CountryCode, @ReportType, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                pMessageID, pCommsMethod, pCountryCode, pReportType, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pCompanyKey).ToListAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<DeliveryOutput>> NoAppUser(int CompanyID, int MessageID, int RecordStart, int RecordLength, string SearchString, string OrderBy, string OrderDir, string CompanyKey) {
            try {


                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString.Trim());
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pCompanyKey = new SqlParameter("@UniqueKey", CompanyKey);

                var result = await _context.Set<DeliveryOutput>().FromSqlRaw("exec Users_Without_App @CompanyID, @MessageID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir,@UniqueKey",
                pCompanyID, pMessageID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pCompanyKey).ToListAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<UserItems>> OffDutyReport(int CompanyId, int UserID) {

            try {

                var pCompanyId = new SqlParameter("@CompanyId", CompanyId);
                var pUserID = new SqlParameter("@UserId", UserID);
                var result = await _context.Set<UserItems>().FromSqlRaw("exec Pro_Report_Off_Duty @CompanyId, @UserId", pCompanyId, pUserID).ToListAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }

        }
        public async Task<string> ExportAcknowledgement(int MessageID, int CompanyID, int UserID, string CompanyKey) {
            try {
                string FileName = "Ack_Report_" + MessageID + ".csv";

                string ResultFilePath = Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + CompanyID + "\\DataExport\\";
                string FilePath = ExportPath + FileName;

                if (!Directory.Exists(ExportPath)) {
                    Directory.CreateDirectory(ExportPath);
                    await DeleteOldFiles(ExportPath);
                }

                if (File.Exists(FilePath)) {
                    File.Delete(FilePath);
                }

                string headerRow = string.Empty;



                var pMessageId = new SqlParameter("@MessageID", MessageID);
                var pMessageAckStatus = new SqlParameter("@MessageAckStatus", 2);
                var pMessageSentStatus = new SqlParameter("@MessageSentStatus", 1);
                var pUserID = new SqlParameter("@UserID", UserID);
                var pSource = new SqlParameter("@Source", "WEB");
                var pRecordStart = new SqlParameter("@RecordStart", "0");
                var pRecordLength = new SqlParameter("@RecordLength", int.MaxValue);
                var pSearchString = new SqlParameter("@SearchString", "");
                var pOrderBy = new SqlParameter("@OrderBy", "ML.DateAcknowledge");
                var pOrderDir = new SqlParameter("@OrderDir", "desc");
                var pFilters = new SqlParameter("@Filters", ",,");
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

                var ack_list = await _context.Set<MessageAcknowledgements>().FromSqlRaw("exec Pro_Get_Message_Acknowledgements @MessageID, @MessageAckStatus, @MessageSentStatus, @UserID,@Source, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @Filters, @UniqueKey",
                   pMessageId, pMessageAckStatus, pMessageSentStatus, pUserID, pSource, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pFilters, pUniqueKey).ToListAsync();

                using (StreamWriter SW = new StreamWriter(FilePath, false)) {

                    headerRow = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                   "User Name", "Email", "Mobile", "Date Sent", "Ack", "Response", "Response Time", "Ack Method");

                    SW.WriteLine(headerRow);

                    foreach (var row in ack_list) {
                        double time_elapsed = (row.MessageAcknowledge.Subtract(row.MessageSent).TotalSeconds);
                        string time_ack = time_elapsed > 0 ? CrisesControl.SharedKernel.Utils.StringExtensions.SectoTime((int)time_elapsed) : "-";
                        string acktime = row.MessageAcknowledge.Year > 2000 ? row.MessageAcknowledge.ToString("dd-MMM-yyyy HH:mm:ss") : "-";
                        string rsplabel = !string.IsNullOrEmpty(row.ResponseLabel) ? row.ResponseLabel : row.MessageAckStatus == 1 ? "Acknowledged" : "-";
                        string ackmethod = !string.IsNullOrEmpty(row.AckMethod) ? row.AckMethod : "";

                        string rowdata = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                        row.FirstName + " " + row.LastName, row.UserEmail, row.ISDCode + row.MobileNo, row.MessageSent.ToString("dd-MMM-yyyy HH:mm:ss"), acktime, rsplabel, time_ack, ackmethod);
                        if (!string.IsNullOrEmpty(rowdata))
                            SW.WriteLine(rowdata);
                    }
                }
                return FileName;



            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<IncidentResponseSummary>> IncidentResponseSummary(int ActiveIncidentID) {
            try {


                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
                var result = await _context.Set<IncidentResponseSummary>().FromSqlRaw("exec Message_Response_Summary @ActiveIncidentID", pActiveIncidentID).ToListAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public DataTable GetReportData(int ActiveIncidentID, out string rFilePath, out string rFileName) {
            DataTable dt = new DataTable();
            rFilePath = string.Empty;
            rFileName = string.Empty;
            try {

                string ResultFilePath = Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + "DataExport\\";

                connectUNCPath();

                var ReportSP = "Get_Active_Incident_Response_Summary";
                string FileName = "Incident_Response_" + ActiveIncidentID + ".csv";

                string FilePath = ExportPath + FileName;
                rFilePath = FilePath;
                rFileName = FileName;

                if (!Directory.Exists(ExportPath)) {
                    Directory.CreateDirectory(ExportPath);
                    DeleteOldFiles(ExportPath);
                }

                if (File.Exists(FilePath)) {
                    File.Delete(FilePath);
                }

                string constr = configuration.GetConnectionString("CrisesControlDatabase");
                using (SqlConnection con = new SqlConnection(constr)) {
                    ReportSP += " ";
                    ReportSP += "@ActiveIncidentID";

                    using (SqlCommand cmd = new SqlCommand(ReportSP)) {
                        cmd.Parameters.AddWithValue("@ActiveIncidentID", ActiveIncidentID);

                        using (SqlDataAdapter sda = new SqlDataAdapter()) {
                            cmd.Connection = con;
                            con.Open();
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                        }
                    }
                }
                return dt;
            } catch (Exception ex) {
                throw ex;
                return dt;
            }
        }

        public async Task AppInvitation(int CompanyID) {
            try {
                //await _queueService.AppInvitationQueue(CompanyID);
            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<bool> connectUNCPath(string UNCPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "") {
            try {
                if (!string.IsNullOrEmpty(UNCPath))
                    UNCPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UNCPath"]);
                if (!string.IsNullOrEmpty(UseUNC))
                    UseUNC = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UseUNC"]);
                if (!string.IsNullOrEmpty(strUncUsername))
                    strUncUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncUserName"]);
                if (!string.IsNullOrEmpty(strUncPassword))
                    strUncPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncPassword"]);

                if (UseUNC == "true") {
                    UNCAccessWithCredentials.disconnectRemote(@UNCPath);
                    if (string.IsNullOrEmpty(UNCAccessWithCredentials.connectToRemote(@UNCPath, strUncUsername, strUncPassword))) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } catch (Exception ex) {
                throw ex;
                return false;
            }
            return true;
        }
        public async Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",") {

            StringBuilder csvRows = new StringBuilder();
            string row = "";
            int columns;
            try {
                //dataTable.Load(dataReader);
                columns = dataTable.Columns.Count;
                //Create Header
                if (includeHeaderAsFirstRow) {
                    for (int index = 0; index < columns; index++) {
                        row += (dataTable.Columns[index]);
                        if (index < columns - 1)
                            row += (separator);
                    }
                    row += (Environment.NewLine);
                }
                csvRows.Append(row);

                //Create Rows
                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++) {
                    row = "";
                    //Row
                    for (int index = 0; index < columns; index++) {
                        string value = dataTable.Rows[rowIndex][index].ToString();

                        //If type of field is string
                        if (dataTable.Rows[rowIndex][index] is string) {
                            //If double quotes are used in value, ensure each are replaced by double quotes.
                            if (value.IndexOf("\"") >= 0)
                                value = value.Replace("\"", "\"\"");

                            //If separtor are is in value, ensure it is put in double quotes.
                            if (value.IndexOf(separator) >= 0)
                                value = "\"" + value + "\"";

                            //If string contain new line character
                            while (value.Contains("\r")) {
                                value = value.Replace("\r", "");
                            }
                            while (value.Contains("\n")) {
                                value = value.Replace("\n", "");
                            }
                        }
                        row += value;
                        if (index < columns - 1)
                            row += separator;
                    }
                    dataTable.Rows[rowIndex][columns - 1].ToString().Replace(separator, " ");
                    row += Environment.NewLine;
                    csvRows.Append(row);
                }
            } catch (Exception ex) {
                throw ex;
            }
            dataTable.Dispose();
            return csvRows.ToString();
        }
        public async Task<DataTablePaging> GetMessageAnslysisResponse(int companyId, int MessageId, string MessageType, int DrillOpt, int start, int length, string search, string orderBy, string orderDir, string CompanyKey, int draw) {
            try {

                var RecordStart = start == 0 ? 0 : start;
                var RecordLength = length == 0 ? int.MaxValue : length;
                var SearchString = (search != null) ? search : String.Empty;
                string OrderBy = orderBy != null ? orderBy.FirstOrDefault().ToString() : "DateSent";
                string OrderDir = orderDir != null ? orderDir.FirstOrDefault().ToString() : "desc";

                var returnData = await GetMessageAnalysis(companyId, MessageId, MessageType, DrillOpt, RecordStart, RecordLength, SearchString, OrderBy, OrderDir, CompanyKey);

                int totalRecord = 0;

                if (returnData != null)
                    totalRecord = returnData.Count;

                List<PingReportGrid> ttodata = await GetMessageAnalysis(companyId, MessageId, MessageType, DrillOpt, 0, int.MaxValue, "", "MessageListId", OrderDir, CompanyKey);

                if (ttodata != null)
                    totalRecord = ttodata.Count;


                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = draw;
                rtn.RecordsTotal = totalRecord;
                rtn.RecordsFiltered = returnData.Count();
                rtn.Data = returnData;


                return rtn;


            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<PingReportGrid>> GetMessageAnalysis(int _CompanyID, int MessageId, string MessageType, int DrillOpt, int RecordStart, int RecordLength, string SearchString,
            string OrderBy, string OrderDir, string CompanyKey) {
            try {


                var pMessageId = new SqlParameter("@MessageID", MessageId);
                var pCompanyID = new SqlParameter("@CompanyID", _CompanyID);
                var pMessageType = new SqlParameter("@MessageType", MessageType);
                var pDrillOption = new SqlParameter("@DrillOption", DrillOpt);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

                var result = new List<PingReportGrid>();
                var propertyInfo = typeof(PingReportGrid).GetProperty(OrderBy);

                if (OrderDir == "desc") {
                    result = await _context.Set<PingReportGrid>().FromSqlRaw(" exec Pro_Report_Ping_Analysis_Response @MessageID, @CompanyID, @MessageType, @DrillOption, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                        pMessageId, pCompanyID, pMessageType, pDrillOption, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToListAsync();
                    result.Select(c => {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    })
                    .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
                } else {
                    result = await _context.Set<PingReportGrid>().FromSqlRaw("exec Pro_Report_Ping_Analysis_Response @MessageID, @CompanyID, @MessageType, @DrillOption, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                        pMessageId, pCompanyID, pMessageType, pDrillOption, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                        .ToListAsync();
                    result.Select(c => {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    })
                    .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
                }


                return result;

            } catch (Exception ex) {
                throw ex;
                return null;
            }
        }

        public async Task<CompanyCountReturn> GetCompanyCommunicationReport(int companyId) {
            var recCompanyCountReturn = new CompanyCountReturn();
            try {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                recCompanyCountReturn = await _context.Set<CompanyCountReturn>().FromSqlRaw("EXEC Pro_Report_GetCompanyCommunicationReport @CompanyID", pCompanyID).FirstOrDefaultAsync();
                if (recCompanyCountReturn != null) {
                    var companyID2 = new SqlParameter("@CompanyID", companyId);
                    recCompanyCountReturn.CompanyUserCountReturn = _context.Set<CompanyUserCountReturn>().FromSqlRaw("EXEC Pro_Report_GetCompanyUserCommunicationReport @CompanyID", companyID2).ToList();
                } else {
                    recCompanyCountReturn.ErrorId = 110;
                    recCompanyCountReturn.Message = "No record found.";
                }
                return recCompanyCountReturn;
            } catch (Exception ex) {
                throw ex;
            }

        }

        public async Task<List<TrackingExport>> GetUserTracking(string source, int userId, int activeIncidentId) {
            try {
                var pSource = new SqlParameter("@Source", source);
                var pUserID = new SqlParameter("@UserID", userId);
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", activeIncidentId);

                var tckusr = await _context.Set<TrackingExport>().FromSqlRaw("EXEC Pro_Get_User_Tracking @Source, @UserID,  @ActiveIncidentID",
                    pSource, pUserID, pActiveIncidentID).ToListAsync();
                var res = tckusr.Select(c => {
                    c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    return c;
                }).ToList();

                return res;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<TaskOverview> CMD_TaskOverView(int activeIncidentId) {
            try {
                var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", activeIncidentId);
                var result = await _context.Set<TaskOverview>().FromSqlRaw("EXEC Pro_ICC_Task_Overview @ActiveIncidentID", pActiveIncidentID).FirstOrDefaultAsync();
                return result!;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<DataTablePaging> GetUserInvitationReport(UserInvitationModel userInvitation) {
            try {
                var tablePaging = new DataTablePaging();

                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                TimeSpan timespan = TimeSpan.FromMinutes(30);
                _DBC.GetStartEndDate(userInvitation.IsThisWeek, userInvitation.IsThisMonth, userInvitation.IsLastMonth, ref stDate, ref enDate, userInvitation.StartDate, userInvitation.EndDate);

                var RecordStart = userInvitation.Start == 0 ? 0 : userInvitation.Start;
                var RecordLength = userInvitation.Length == 0 ? int.MaxValue : userInvitation.Length;
                var SearchString = (userInvitation.Search != null) ? userInvitation.Search.Value : "";
                string OrderBy = userInvitation.Order != null ? userInvitation.Order : "DateSent";
                string OrderDir = userInvitation.Dir != null ? userInvitation.Dir : "desc";

                var returnData = await GetUserInvitationData(userInvitation.CompanyId, userInvitation.CurrentUserId, stDate, enDate, RecordStart, RecordLength, SearchString, OrderBy, OrderDir, userInvitation.CompanyKey);

                int totalRecord = 0;

                if (returnData != null)
                    totalRecord = returnData.Count;

                List<UserInvitationResult> ttodata = await GetUserInvitationData(userInvitation.CompanyId, userInvitation.CurrentUserId, stDate, enDate, 0, int.MaxValue, "", "UserId", OrderDir, userInvitation.CompanyKey);

                if (ttodata != null)
                    totalRecord = ttodata.Count;


                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = userInvitation.Draw;
                rtn.RecordsTotal = totalRecord;
                rtn.RecordsFiltered = returnData.Count;
                rtn.Data = returnData;

                if (rtn == null) {
                    tablePaging.ErrorId = 110;
                    tablePaging.Message = "No record found.";
                }
                return tablePaging;
            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<List<UserInvitationResult>> GetUserInvitationData(int companyId, int userId, DateTime startDate, DateTime endDate, int recordStart, int recordLength, string searchString, string orderBy, string orderDir, string companyKey) {
            try {
                var pStartDate = new SqlParameter("@StartDate", startDate);
                var pEndDate = new SqlParameter("@EndDate", endDate);
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
                var pUserId = new SqlParameter("@UserId", userId);
                var pRecordStart = new SqlParameter("@RecordStart", recordStart);
                var pRecordLength = new SqlParameter("@RecordLength", recordLength);
                var pSearchString = new SqlParameter("@SearchString", searchString);
                var pOrderBy = new SqlParameter("@OrderBy", orderBy);
                var pOrderDir = new SqlParameter("@OrderDir", orderDir);
                var pUniqueKey = new SqlParameter("@UniqueKey", companyKey);

                var result = new List<UserInvitationResult>();
                var propertyInfo = typeof(UserInvitationResult).GetProperty(orderBy);

                if (orderDir == "desc") {
                    result = await _context.Set<UserInvitationResult>().FromSqlRaw("EXEC Pro_Get_User_Invitation @CompanyID, @UserId,@StartDate,@EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                        pCompanyId, pUserId, pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey).OrderByDescending(o => propertyInfo.GetValue(o, null)).ToListAsync();
                } else {
                    result = await _context.Set<UserInvitationResult>().FromSqlRaw("EXEC Pro_Get_User_Invitation @CompanyID, @UserId,@StartDate,@EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                        pCompanyId, pUserId, pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey).OrderBy(o => propertyInfo.GetValue(o, null)).ToListAsync();
                }

                return result;
            } catch (Exception ex) {
                return null;
            }
        }

        public DataTable GetUserInvitationReportData(UserInvitationModel inputModel, out string rFilePath, out string rFileName) {
            DataTable dt = new DataTable();
            rFilePath = string.Empty;
            rFileName = string.Empty;
            try {

                string ResultFilePath = _DBC.Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + inputModel.CompanyId.ToString() + "\\DataExport\\";

                _DBC.connectUNCPath();

                var ReportSP = "Pro_Get_User_Invitation";
                string FileName = "User_Invitation_Report" + DateTime.Now.ToString("ddmmyyyyhhss") + ".csv";

                string FilePath = ExportPath + FileName;
                rFilePath = FilePath;
                rFileName = FileName;

                if (!Directory.Exists(ExportPath)) {
                    Directory.CreateDirectory(ExportPath);
                    _DBC.DeleteOldFiles(ExportPath);
                }

                if (File.Exists(FilePath)) {
                    File.Delete(FilePath);
                }

                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                TimeSpan timespan = TimeSpan.FromMinutes(30);
                _DBC.GetStartEndDate(inputModel.IsThisWeek, inputModel.IsThisMonth, inputModel.IsLastMonth, ref stDate, ref enDate, inputModel.StartDate, inputModel.EndDate);

                var RecordStart = inputModel.Start == 0 ? 0 : inputModel.Start;
                var RecordLength = inputModel.Length == 0 ? int.MaxValue : inputModel.Length;
                var SearchString = (inputModel.Search != null) ? inputModel.Search.Value : "";
                string OrderBy = inputModel.Order != null ? inputModel.Order : "DateSent";
                string OrderDir = inputModel.Dir != null ? inputModel.Dir : "desc";


                string constr = _context.Database.GetConnectionString()!;
                using (SqlConnection con = new SqlConnection(constr)) {
                    ReportSP += " ";
                    ReportSP += "@CompanyID, @UserId, @StartDate,@EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey";

                    using (SqlCommand cmd = new SqlCommand(ReportSP)) {
                        cmd.Parameters.AddWithValue("@CompanyID", inputModel.CompanyId);
                        cmd.Parameters.AddWithValue("@UserId", inputModel.CurrentUserId);
                        cmd.Parameters.AddWithValue("@StartDate", stDate);
                        cmd.Parameters.AddWithValue("@EndDate", enDate);
                        cmd.Parameters.AddWithValue("@RecordStart", 0);
                        cmd.Parameters.AddWithValue("@RecordLength", int.MaxValue);
                        cmd.Parameters.AddWithValue("@SearchString", SearchString);
                        cmd.Parameters.AddWithValue("@OrderBy", OrderBy);
                        cmd.Parameters.AddWithValue("@OrderDir", OrderDir);
                        cmd.Parameters.AddWithValue("@UniqueKey", inputModel.CompanyId);

                        using (SqlDataAdapter sda = new SqlDataAdapter()) {
                            cmd.Connection = con;
                            con.Open();
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                        }
                    }
                }
                return dt;
            } catch (Exception ex) {
                return dt;
            }
        }
    }
}
