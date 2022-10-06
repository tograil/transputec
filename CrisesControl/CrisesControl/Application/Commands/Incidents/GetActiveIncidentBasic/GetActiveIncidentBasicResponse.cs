using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentBasic
{
    public class GetActiveIncidentBasicResponse
    {
        public List<UpdateIncident> result { get; set; }
    }
}
