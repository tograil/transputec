using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocations;

namespace CrisesControl.Api.Application.Query
{
    public interface ILocationQuery
    {
        public Task<GetLocationsResponse> GetLocations(GetLocationsRequest request);
        public Task<GetLocationResponse> GetLocation(GetLocationRequest request);
    }
}
