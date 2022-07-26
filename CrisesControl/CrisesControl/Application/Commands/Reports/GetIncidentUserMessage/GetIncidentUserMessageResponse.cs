using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentUserMessage
{
    public class GetIncidentUserMessageResponse
    {
        public List<IncidentUserMessageResponse> data { get; set; }
        public string Message { get; set; }
    }
}
