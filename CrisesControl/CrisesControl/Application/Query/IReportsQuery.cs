using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.SP_Response;

namespace CrisesControl.Api.Application.Query {
    public interface IReportsQuery {
        public Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request);
        public Task<GetIncidentPingStatsResponse> GetIncidentPingStats(GetIncidentPingStatsRequest request);
        Task<GetIndidentMessageAckResponse> GetIndidentMessageAck(GetIndidentMessageAckRequest request);
        Task<ResponseSummaryResponse> ResponseSummary(ResponseSummaryRequest request);
        Task<GetIndidentMessageNoAckResponse> GetIndidentMessageNoAck(GetIndidentMessageNoAckRequest request);
        GetCurrentIncidentStatsResponse GetCurrentIncidentStats();
        IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId);
    }
}
