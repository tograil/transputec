using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Core.Locations;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocations
{
    public class GetLocationsResponse
    {
        public List<LocationDetail> Data { get; set; }
    }
}
