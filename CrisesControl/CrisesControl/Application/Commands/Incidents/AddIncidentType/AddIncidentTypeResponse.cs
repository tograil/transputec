using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.AddIncidentType
{
    public class AddIncidentTypeResponse
    {
        public int TypeID { get; set; }
        public List<IncidentTypeReturn> incidentTypes { get; set; }
        public string Message { get; set; }
    }
}
