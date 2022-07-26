using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetPerformanceReport
{
    public class GetPerformanceReportRequest:IRequest<GetPerformanceReportResponse>
    {
        public GroupType GroupType { get; set; }
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public bool ShowDeletedGroups { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

    }
}
