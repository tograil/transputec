using Ardalis.GuardClauses;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Core.LocationAggregate;
using CrisesControl.Core.LocationAggregate.Services;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.CreateLocation
{
    public class CreateLocationHandler: IRequestHandler<CreateLocationRequest, CreateLocationResponse>
    {
        private readonly CreateLocationValidator _locationValidator;
        private readonly ILocationRepository _locationRepository;

        public CreateLocationHandler(CreateLocationValidator locationValidator, ILocationRepository locationService)
        {
            _locationValidator = locationValidator;
            _locationRepository = locationService;
        }

        public async Task<CreateLocationResponse> Handle(CreateLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateLocationRequest));

            var sample = new Location();
            var locationId = await _locationRepository.CreateLocation(sample, cancellationToken);
            var result = new CreateLocationResponse();
            result.LocationId = locationId;

            return result;
        }
    }
}
