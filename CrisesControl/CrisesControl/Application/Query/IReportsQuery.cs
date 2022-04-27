using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;

namespace CrisesControl.Api.Application.Query {
    public interface IReportsQuery {
        public Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request);
        public Task<GetIncidentPingStatsResponse> GetIncidentPingStats(GetIncidentPingStatsRequest request);
    }
}
