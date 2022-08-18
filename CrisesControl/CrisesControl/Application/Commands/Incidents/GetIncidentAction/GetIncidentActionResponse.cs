using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentAction
{
    public class GetIncidentActionResponse
    {
        public List<ActionLsts> Data { get; set; }
        public string Message { get; set; }
    }
}
