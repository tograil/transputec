using Ardalis.GuardClauses;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public CreateLocationHandler(CreateLocationValidator locationValidator, ILocationRepository locationService, IMapper mapper)
        {
            _locationValidator = locationValidator;
            _locationRepository = locationService;
            _mapper = mapper;
        }

        public async Task<CreateLocationResponse> Handle(CreateLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateLocationRequest));

            Location value = _mapper.Map<CreateLocationRequest, Location>(request);
            if (CheckDuplicate(value))
            {
                var locationId = await _locationRepository.CreateLocation(value, cancellationToken);
                var result = new CreateLocationResponse();
                result.LocationId = locationId;

                return result;
            }
            return null;
        }

        private bool CheckDuplicate(Location location)
        {
            return _locationRepository.CheckDuplicate(location);
        }
    }
}
