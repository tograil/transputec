using CrisesControl.Api.Application.Commands.Locations.DeleteLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocations;

namespace CrisesControl.Api.Application.Query
{
    public interface ILocationQuery
    {
        public Task<GetLocationsResponse> GetLocations(GetLocationsRequest request, CancellationToken cancellationToken);
        public Task<GetLocationResponse> GetLocation(GetLocationRequest request, CancellationToken cancellationToken);
        Task<DeleteLocationResponse> DeleteLocation(DeleteLocationRequest request);
    }
}
