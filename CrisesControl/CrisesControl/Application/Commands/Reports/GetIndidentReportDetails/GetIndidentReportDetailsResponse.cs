using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentReportDetails
{
    public class GetIndidentReportDetailsResponse
    {
        public List<IncidentMessagesRtn> data { get; set; }
        public string Message { get; set; }
    }
}
