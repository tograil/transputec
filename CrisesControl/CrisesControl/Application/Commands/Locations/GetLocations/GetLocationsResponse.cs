using CrisesControl.Api.Application.Commands.Locations.GetLocation;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocations
{
    public class GetLocationsResponse
    {
        public List<GetLocationResponse> Data { get; set; }
    }
}
