using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetGroupPingReportChart
{
    public class GetGroupPingReportChartResponse
    {
        public PingGroupChartCount data { get; set; }
        public string Message { get; set; }
    }
}
