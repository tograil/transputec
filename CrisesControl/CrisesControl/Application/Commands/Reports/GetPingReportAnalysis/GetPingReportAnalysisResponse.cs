using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportAnalysis
{
    public class GetPingReportAnalysisResponse
    {
        public PingReport Data { get; set; }
        public string Message { get; set; }
    }
}
