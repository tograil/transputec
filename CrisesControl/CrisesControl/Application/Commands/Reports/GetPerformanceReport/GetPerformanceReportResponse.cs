using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetPerformanceReport
{
    public class GetPerformanceReportResponse
    {
        public List<PerformanceReport> Data { get; set; }
        public string Message { get; set; }
    }
}
