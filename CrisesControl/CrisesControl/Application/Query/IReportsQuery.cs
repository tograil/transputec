using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;

namespace CrisesControl.Api.Application.Query {
    public interface IReportsQuery {
        public Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request);
        public Task<GetIncidentPingStatsResponse> GetIncidentPingStats(GetIncidentPingStatsRequest request);
        Task<GetIndidentMessageAckResponse> GetIndidentMessageAck(GetIndidentMessageAckRequest request);
        Task<ResponseSummaryResponse> ResponseSummary(ResponseSummaryRequest request);
        Task<GetIndidentMessageNoAckResponse> GetIndidentMessageNoAck(GetIndidentMessageNoAckRequest request);
        Task<GetMessageDeliveryReportResponse> GetMessageDeliveryReport(GetMessageDeliveryReportRequest request);
    }
}
