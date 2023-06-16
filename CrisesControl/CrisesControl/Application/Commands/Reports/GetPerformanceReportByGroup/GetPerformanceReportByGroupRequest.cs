using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetPerformanceReportByGroup
{
    public class GetPerformanceReportByGroupRequest:IRequest<GetPerformanceReportByGroupResponse>
    {
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public MethodType MessageType { get; set; } 
        public string GroupName { get; set; }
        public GroupType GroupType { get; set; }
        public int DrillOpt { get; set; }
        public string SearchString { get; set; } = "";
        public string CompanyKey { get; set; } = "";
        public int  draw { get; set; }
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
 
    }
}
