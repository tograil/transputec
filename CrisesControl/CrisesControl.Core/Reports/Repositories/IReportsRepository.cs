using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports.SP_Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.Repositories
{
    public interface IReportsRepository {
        public Task<List<SOSItem>> GetSOSItems();
        public Task<List<IncidentPingStatsCount>> GetIncidentPingStats(int CompanyID, int NoOfMonth);
        Task<List<ResponseSummary>> ResponseSummary(int MessageID);
        Task<DataTablePaging> GetIndidentMessageAck(int MessageId, int MessageAckStatus, int MessageSentStatus, int RecordStart, int RecordLength, string search,
             string OrderBy, string OrderDir, int draw, string Filters, string CompanyKey, string Source = "WEB");
        Task<List<DataTablePaging>> GetIndidentMessageNoAck(int draw, int IncidentActivationId
        ,int RecordStart, int RecordLength, string? SearchString, string? UniqueKey);
        CurrentIncidentStatsResponse GetCurrentIncidentStats(string timeZoneId, int companyId);
        IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId);
        Task<List<PingGroupChartCount>> GetPingReportChart(DateTime StartDate, DateTime EndDate, int GroupID, string MessageType, int ObjectMappingID);
        Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
        Task<List<DeliveryOutput>> GetMessageDeliveryReport(DateTimeOffset StartDate, DateTimeOffset EndDate, int start, int length, string search, string OrderBy,string OrderDir, string CompanyKey);
        //DataTablePaging GetResponseReportByGroup(int companyId, DateTimeOffset startDate, DateTimeOffset endDate, string messageType, int drillOpt, int groupId, int objectMappingId, string companyKey, bool isThisWeek, bool isThisMonth, bool isLastMonth, int recordStart, int recordLength, string? searchString, string? orderBy, string? orderDir, Search search, List<Order> order);
        DataTablePaging GetResponseReportByGroup(DataTableAjaxPostModel dtapm, DateTimeOffset startDate, DateTimeOffset endDate, string messageType, int drillOpt, int groupId, int objectMappingId, string companyKey, bool isThisWeek, bool isThisMonth, bool isLastMonth, int companyId);
        List<IncidentMessageAuditResponse> GetIndidentMessagesAudit(int incidentActivationId, int companyId);
        List<IncidentUserLocationResponse> GetIncidentUserLocation(int incidentActivationId, int companyId);
        List<TrackUsers> GetTrackingUsers(string status, int userId, int companyId);
        Task<List<TrackUserCount>> GetTrackingUserCount(int companyId);
        Task<dynamic> GetMessageDeliverySummary(int MessageID);
    }
}
