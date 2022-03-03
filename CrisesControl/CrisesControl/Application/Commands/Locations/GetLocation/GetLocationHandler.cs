using Ardalis.GuardClauses;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Core.LocationAggregate.Services;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocation
{
    public class GetLocationHandler: IRequestHandler<GetLocationRequest, GetLocationResponse>
    {
        private readonly GetLocationValidator _locationValidator;
        private readonly ILocationRepository _locationRepository;

        public GetLocationHandler(GetLocationValidator locationValidator, ILocationRepository locationRepository)
        {
            _locationValidator = locationValidator;
            _locationRepository = locationRepository;
        }

        public async Task<GetLocationResponse> Handle(GetLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLocationRequest));
            
            await _locationValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _locationRepository.GetAllLocations();

            return new GetLocationResponse();
        }
    }
}
