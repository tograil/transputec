using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAffectedLocations
{
    public class GetAffectedLocationsResponse
    {
        public List<AffectedLocation> Data { get; set; }
    }
}
