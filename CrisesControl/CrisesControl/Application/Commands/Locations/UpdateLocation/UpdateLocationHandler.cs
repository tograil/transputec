using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using CrisesControl.SharedKernel.Utils;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateLocation
{
    public class UpdateLocationHandler: IRequestHandler<UpdateLocationRequest, UpdateLocationResponse>
    {
        private readonly UpdateLocationValidator _locationValidator;
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<UpdateLocationHandler> _logger;
        private readonly ICurrentUser _currentUser;

        public UpdateLocationHandler(UpdateLocationValidator locationValidator, ILocationRepository locationRepository, ILogger<UpdateLocationHandler> logger, ICurrentUser currentUser)
        {
            _locationValidator = locationValidator;
            _locationRepository = locationRepository;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<UpdateLocationResponse?> Handle(UpdateLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateLocationRequest));

            Location value = new Location() //_mapper.Map<UpdateLocationRequest, Location>(request);
            {
                LocationId=request.LocationId,
                CompanyId = request.CompanyId,
                Desc = request.Desc,               
                Lat = request.Lat,
                LocationName = request.LocationName,
                Long = request.Long,
                PostCode = request.PostCode,
                Status = 3,
                UpdatedBy = _currentUser.UserId,
                UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
            };
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
