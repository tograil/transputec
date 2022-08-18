using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetSOSIncident
{
    public class GetSOSIncidentResponse
    {
        public IncidentDetails Data { get; set; }
        public string Message { get; set; }
    }
}
