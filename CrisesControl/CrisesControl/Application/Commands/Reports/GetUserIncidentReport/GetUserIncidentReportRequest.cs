using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserIncidentReport
{
    public class GetUserIncidentReportRequest:IRequest<GetUserIncidentReportResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
