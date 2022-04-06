using Ardalis.GuardClauses;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UpdateLocationHandler(UpdateLocationValidator locationValidator, ILocationRepository locationRepository, IMapper mapper)
        {
            _locationValidator = locationValidator;
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<UpdateLocationResponse?> Handle(UpdateLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateLocationRequest));

            Location value = _mapper.Map<UpdateLocationRequest, Location>(request);

            if (CheckForExistance(value))
            {
                var locationId = await _locationRepository.UpdateLocation(value, cancellationToken);
                var result = new UpdateLocationResponse();
                result.LocationId = locationId;
                return result;
            }

            return null;
        }

        private bool CheckForExistance(Location request)
        {
            return _locationRepository.CheckForExisting(request.LocationId);
        }
    }
}
