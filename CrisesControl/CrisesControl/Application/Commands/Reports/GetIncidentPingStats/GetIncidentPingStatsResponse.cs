using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats {
    public class GetIncidentPingStatsResponse {
        public List<IncidentPingStatsCount> Data { get; set; }
        public string ErrorCode { get; set; }

    }
}
