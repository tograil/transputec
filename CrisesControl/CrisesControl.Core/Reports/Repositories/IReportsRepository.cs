using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports.SP_Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.Repositories
{
    public interface IReportsRepository {
        public Task<List<SOSItem>> GetSOSItems(int UserId);
        public Task<List<IncidentPingStatsCount>> GetIncidentPingStats(int CompanyID, int NoOfMonth);
        Task<List<ResponseSummary>> ResponseSummary(int MessageID);
        Task<DataTablePaging> GetIncidentMessageAck(int MessageId, int MessageAckStatus, int MessageSentStatus, string Source, int RecordStart, int RecordLength,
            string SearchString = "", string OrderBy = "DateAcknowledge", string OrderDir = "asc", string Filters = "", string UniqueKey = "");

        Task<List<MessageAcknowledgements>> GetAcknowledgements(int MessageId, int MessageAckStatus, int MessageSentStatus, int RecordStart, int RecordLength, string search,
             string OrderBy, string OrderDir, string Filters, string CompanyKey, string Source = "WEB");
        Task<List<DataTablePaging>> GetIncidentMessageNoAck(int draw, int IncidentActivationId
        ,int RecordStart, int RecordLength, string? SearchString, string? UniqueKey);
        CurrentIncidentStatsResponse GetCurrentIncidentStats(string timeZoneId, int companyId);
        IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId);
        Task<List<PingGroupChartCount>> GetPingReportChart(DateTime StartDate, DateTime EndDate, int GroupID, string MessageType, int ObjectMappingID);
        Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
        Task<List<DeliveryOutput>> GetMessageDeliveryReport(DateTimeOffset StartDate, DateTimeOffset EndDate, int start, int length, string search, string OrderBy,string OrderDir, string CompanyKey);
        //DataTablePaging GetResponseReportByGroup(int companyId, DateTimeOffset startDate, DateTimeOffset endDate, string messageType, int drillOpt, int groupId, int objectMappingId, string companyKey, bool isThisWeek, bool isThisMonth, bool isLastMonth, int recordStart, int recordLength, string? searchString, string? orderBy, string? orderDir, Search search, List<Order> order);
        DataTablePaging GetResponseReportByGroup(DataTableAjaxPostModel dtapm, DateTimeOffset startDate, DateTimeOffset endDate, string messageType, int drillOpt, int groupId, int objectMappingId, string companyKey, bool isThisWeek, bool isThisMonth, bool isLastMonth, int companyId);
        List<IncidentMessageAuditResponse> GetIncidentMessagesAudit(int incidentActivationId, int companyId);
        List<IncidentUserLocationResponse> GetIncidentUserLocation(int incidentActivationId, int companyId);
        List<TrackUsers> GetTrackingUsers(string status, int userId, int companyId);
        Task<List<TrackUserCount>> GetTrackingUserCount(int companyId);
        Task<dynamic> GetMessageDeliverySummary(int MessageID);
        Task<List<IncidentMessagesRtn>> GetIncidentReportDetails(int IncidentActivationID, int CompanyID, int UserId);
        Task<DataTablePaging> GetIncidentReport(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate, int SelectedUserID, int CompanyId);
        Task<List<UserPieChart>> GetUserReportPiechartData(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate, int SelectedUserID, int CompanyId);
        Task<List<UserIncidentReportResponse>> GetUserIncidentReport(DateTimeOffset startDate, DateTimeOffset endDate, bool isThisWeek, bool isThisMonth, bool isLastMonth, int SelectedUserID);
        void GetStartEndDate(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, ref DateTime stDate, ref DateTime enDate, DateTimeOffset StartDate, DateTimeOffset EndDate);
        Task<List<PingReport>> GetPingReportAnalysis(int MessageID, string MessageType, int _CompanyID);
        Task<List<IncidentUserMessageResponse>> GetIncidentUserMessage(int IncidentActivationId, int SelectedUserId, int CompanyID);
        Task<IncidentStatsResponse> GetIncidentStats(int IncidentActivationId, int CompanyId);
        Task<List<PerformanceReport>> GetPerformanceReport(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, bool ShowDeletedGroups, int CurrentUserId, int CompanyId, DateTimeOffset StartDate, DateTimeOffset EndDate, string MessageType);
        Task<List<PingReportGrid>> GetPerformanceReport(DateTime StartDate, DateTime EndDate, string MessageType, int companyId, string GroupName = "", string GroupType = "", int FilterUser = 0,
                int DrillOpt = 0, int RecordStart = 0, int RecordLength = 100, string SearchString = "", string OrderBy = "DateSent", string OrderDir = "desc", string CompanyKey = "");
        Task<List<PingReportGrid>> GetPerformanceReportByGroup(DateTime StartDate, DateTime EndDate, string MessageType, int _CompanyID, string GroupName = "", string GroupType = "", int DrillOpt = 0,
            int RecordStart = 0, int RecordLength = 100, string SearchString = "", string OrderBy = "DateSent", string OrderDir = "desc", string CompanyKey = "");
        Task<DataTablePaging> GetPerformanceReportByGroup(int draw, bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, int CompanyId, int start, int length, string search, string orderby, string dir, int FilterUser, string MessageType, string GroupName, string GroupType, int DrillOpt, DateTimeOffset StartDate, DateTimeOffset EndDate, string CompanyKey = "");
        Task<DataTablePaging> GetResponseCoordinates(int start, int length, int MessageId);
        Task<List<TrackingExport>> GetTrackingData(int TrackMeID, int UserDeviceID, DateTimeOffset StartDate, DateTimeOffset EndDate);
        Task<TaskPerformance> GetTaskPerformance(int companyId, bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate);
        Task<FailedTaskReport> GetFailedTasks(string ReportType, int companyId, bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, DateTimeOffset StartDate, DateTimeOffset EndDate);
        Task<List<FailedTaskList>> GetFailedTaskList(string ReportType, int RangeMin, int RangeMax, DateTime stDate, DateTime enDate, int RecordStart, int RecordLength,
            string SearchString, string OrderBy, string OrderDir, string CompanyKey, int _CompanyID);
        Task<List<FailedAttempts>> GetFailedAttempts(int MessageListID, string CommsMethod);
        Task<List<DeliveryDetails>> GetMessageDeliveryDetails(int CompanyID, int MessageID, int UserID);
        Task<string> DownloadDeliveryReport(int CompanyID, int MessageID, int UserID);
        Task<List<DeliveryOutput>> GetUndeliveredMessage(int MessageID, string CommsMethod, string CountryCode, string ReportType, int RecordStart, int RecordLength,
            string SearchString, string OrderBy, string OrderDir, string CompanyKey);
        Task<List<DeliveryOutput>> NoAppUser(int CompanyID, int MessageID, int RecordStart, int RecordLength, string SearchString, string OrderBy, string OrderDir, string CompanyKey);
        Task<List<UserItems>> OffDutyReport(int CompanyId, int UserID);
        Task<string> ExportAcknowledgement(int MessageID, int CompanyID, int UserID, string CompanyKey);
        Task<List<IncidentResponseSummary>> IncidentResponseSummary(int ActiveIncidentID);
        DataTable GetReportData(int ActiveIncidentID, out string rFilePath, out string rFileName);
        Task<bool> connectUNCPath(string UNCPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "");
        Task AppInvitation(int CompanyID);
        Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",");
        Task<DataTablePaging> GetMessageAnslysisResponse(int companyId, int MessageId, string MessageType, int DrillOpt, int start, int length, string search, string orderBy, string orderDir, string CompanyKey, int draw);
        Task<List<PingReportGrid>> GetMessageAnalysis(int _CompanyID, int MessageId, string MessageType, int DrillOpt, int RecordStart, int RecordLength, string SearchString,
            string OrderBy, string OrderDir, string CompanyKey);
        Task<CompanyCountReturn> GetCompanyCommunicationReport(int companyId);
        Task<List<TrackingExport>> GetUserTracking(string source, int userId, int activeIncidentId);
        Task<TaskOverview> CMD_TaskOverView(int activeIncidentId);
        Task<DataTablePaging> GetUserInvitationReport(UserInvitationModel userInvitation);
        DataTable GetUserInvitationReportData(UserInvitationModel inputModel, out string rFilePath, out string rFileName);
    }
}
