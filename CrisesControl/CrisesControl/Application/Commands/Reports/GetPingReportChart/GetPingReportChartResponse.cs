using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportChart
{
    public class GetPingReportChartResponse
    {
        public int KPILimit { get; set; }
        public int KPIMaxLimit { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
        public string Message { get; set; }

    }
}
