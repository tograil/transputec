using Ardalis.GuardClauses;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Core.LocationAggregate;
using CrisesControl.Core.LocationAggregate.Services;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateLocation
{
    public class UpdateLocationHandler: IRequestHandler<UpdateLocationRequest, UpdateLocationResponse>
    {
        private readonly UpdateLocationValidator _locationValidator;
        private readonly ILocationRepository _locationRepository;

        public UpdateLocationHandler(UpdateLocationValidator locationValidator, ILocationRepository locationRepository)
        {
            _locationValidator = locationValidator;
            _locationRepository = locationRepository;
        }

        public async Task<UpdateLocationResponse> Handle(UpdateLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateLocationRequest));

            var sample = new Location();
            var locationId = await _locationRepository.UpdateLocation(sample, cancellationToken);
            var result = new UpdateLocationResponse();
            result.LocationId = locationId;
            return result;
        }
    }
}
