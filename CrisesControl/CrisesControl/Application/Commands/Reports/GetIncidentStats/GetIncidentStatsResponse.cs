using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentStats
{
    public class GetIncidentStatsResponse
    {
        public IncidentStatsResponse Data { get; set; }
        public string Message { get; set; }
    }
}
