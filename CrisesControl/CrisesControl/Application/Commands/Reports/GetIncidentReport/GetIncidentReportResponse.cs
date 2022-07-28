using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentReport
{
    public class GetIncidentReportResponse
    {
        public DataTablePaging data { get; set; }
        public string Message { get; set; }
    }
}
