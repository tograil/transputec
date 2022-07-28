using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetTaskPerformance
{
    public class GetTaskPerformanceRequest:IRequest<GetTaskPerformanceResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth{ get; set; }
        public bool IsLastMonth { get; set; }
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
