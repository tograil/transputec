using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetGroupPingReportChart
{
    public class GetGroupPingReportChartRequest:IRequest<GetGroupPingReportChartResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int GroupID { get; set; }
        public int ObjectMappingID { get; set; }
        public int FilterRelation { get; set; }
    }
}
