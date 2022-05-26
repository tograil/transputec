using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportChart
{
    public class GetPingReportChartRequest:IRequest<GetPingReportChartResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
