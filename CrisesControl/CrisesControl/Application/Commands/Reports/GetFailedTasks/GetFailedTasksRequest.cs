using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetFailedTasks
{
    public class GetFailedTasksRequest:IRequest<GetFailedTasksResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string dir { get; set; }
        public string  Search { get; set; }
        public int RangeMax { get; set; }
        public int draw { get; set; }
        public string ReportType { get; set; }
        public int RangeMin { get; set; }
        public string CompanyKey { get; set; } = "";

    }
}
