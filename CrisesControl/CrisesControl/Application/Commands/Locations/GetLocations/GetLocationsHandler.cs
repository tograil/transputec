using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Core.GroupAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocations
{
    public class GetLocationsHandler: IRequestHandler<GetLocationsRequest, GetLocationsResponse>
    {
        private readonly GetLocationsValidator _locationValidator;
        private readonly ILocationQuery _locationQuery;

        public GetLocationsHandler(GetLocationsValidator locationValidator, ILocationQuery locationQuery)
        {
            _locationValidator = locationValidator;
            _locationQuery = locationQuery;
        }

        public async Task<GetLocationsResponse> Handle(GetLocationsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLocationsRequest));
            
            await _locationValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var locations = await _locationQuery.GetLocations(request);

            return locations;
        }
    }
}
