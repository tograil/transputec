using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using CrisesControl.SharedKernel.Utils;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.CreateLocation
{
    public class CreateLocationHandler: IRequestHandler<CreateLocationRequest, CreateLocationResponse>
    {
        private readonly CreateLocationValidator _locationValidator;
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<CreateLocationHandler> _logger;
        private readonly ICurrentUser _currentUser;

        public CreateLocationHandler(CreateLocationValidator locationValidator, ILocationRepository locationService, ILogger<CreateLocationHandler> logger, ICurrentUser currentUser)
        {
            _locationValidator = locationValidator;
            _locationRepository = locationService;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<CreateLocationResponse> Handle(CreateLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateLocationRequest));

            Location value = new Location()
            {
                CompanyId = request.CompanyId,
                Desc = request.Desc,
                CreatedBy = _currentUser.UserId,
                Lat = request.Lat,
                CreatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                LocationName = request.LocationName,
                Long = request.Long,
                PostCode = request.PostCode,
                Status = 1,
                UpdatedBy = _currentUser.UserId,
                UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
            }; //_mapper.Map<CreateLocationRequest, Location>(request);
            if (!CheckDuplicate(value))
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
