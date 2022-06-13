using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.GetTrackingUserCount;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Api.Application.Query.Requests;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.SP_Response;

namespace CrisesControl.Api.Application.Query
{
    public interface IReportsQuery {
        public Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request);
        public Task<GetIncidentPingStatsResponse> GetIncidentPingStats(GetIncidentPingStatsRequest request);
        Task<GetIndidentMessageAckResponse> GetIndidentMessageAck(GetIndidentMessageAckRequest request);
        Task<ResponseSummaryResponse> ResponseSummary(ResponseSummaryRequest request);
        Task<GetIndidentMessageNoAckResponse> GetIndidentMessageNoAck(GetIndidentMessageNoAckRequest request);
        CurrentIncidentStatsResponse GetCurrentIncidentStats(int companyId);
        IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId);
        Task<GetMessageDeliveryReportResponse> GetMessageDeliveryReport(GetMessageDeliveryReportRequest request);
        DataTablePaging GetResponseReportByGroup(MessageReportRequest request, int companyId);
        List<IncidentMessageAuditResponse> GetIndidentMessagesAudit(int incidentActivationId, int companyId);
        List<IncidentUserLocationResponse> GetIncidentUserLocation(int incidentActivationId, int companyId);
        List<TrackUsers> GetTrackingUsers(string status, int userId, int companyId);
        Task<GetTrackingUserCountResponse> GetTrackingUserCount(GetTrackingUserCountRequest request);
    }
}
