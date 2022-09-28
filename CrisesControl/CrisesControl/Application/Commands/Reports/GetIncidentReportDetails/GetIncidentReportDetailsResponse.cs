using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentReportDetails
{
    public class GetIncidentReportDetailsResponse
    {
        public List<IncidentMessagesRtn> data { get; set; }
        public string Message { get; set; }
    }
}
