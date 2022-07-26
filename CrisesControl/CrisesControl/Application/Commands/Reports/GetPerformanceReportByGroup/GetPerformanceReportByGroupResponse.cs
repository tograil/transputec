using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Commands.Reports.GetPerformanceReportByGroup
{
    public class GetPerformanceReportByGroupResponse
    {
        public DataTablePaging Data { get; set; }
        public string Message { get; set; }
    }
}
