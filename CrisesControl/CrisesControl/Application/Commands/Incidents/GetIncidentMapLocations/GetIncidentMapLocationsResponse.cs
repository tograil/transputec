using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMapLocations
{
    public class GetIncidentMapLocationsResponse
    {
        public List<MapLocationReturn> Data { get; set; }
    }
}
