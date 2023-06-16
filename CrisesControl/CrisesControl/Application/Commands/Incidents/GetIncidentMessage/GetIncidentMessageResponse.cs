using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMessage
{
    public class GetIncidentMessageResponse
    {
        public List<ActionLsts> Data { get; set; }
        public string Message { get; set; }
    }
}
