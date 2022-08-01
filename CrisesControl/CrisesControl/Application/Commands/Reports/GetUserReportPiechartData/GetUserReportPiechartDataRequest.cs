using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserReportPiechartData
{
    public class GetUserReportPiechartDataRequest:IRequest<GetUserReportPiechartDataResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
