using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocations;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using FluentValidation;

namespace CrisesControl.Api.Application.Query
{
    public class LocationQuery: ILocationQuery
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        private readonly GetLocationValidator _getLocationValidator;
        private readonly GetLocationsValidator _getLocationsValidator;
        public LocationQuery(ILocationRepository locationRepository, IMapper mapper, GetLocationValidator getLocationValidator, GetLocationsValidator getLocationsValidator)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
            _getLocationValidator = getLocationValidator;
            _getLocationsValidator = getLocationsValidator;
        }

        public async Task<GetLocationsResponse> GetLocations(GetLocationsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLocationRequest));
            await _getLocationsValidator.ValidateAndThrowAsync(request, cancellationToken);

            var locations = await _locationRepository.GetAllLocations(request.CompanyId);
            List<GetLocationResponse> response = _mapper.Map<List<Location>, List<GetLocationResponse>>(locations.ToList());
            var result = new GetLocationsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetLocationResponse> GetLocation(GetLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLocationRequest));
            await _getLocationValidator.ValidateAndThrowAsync(request, cancellationToken);

            var location = await _locationRepository.GetLocationById(request.LocationId);
            GetLocationResponse response = _mapper.Map<Location, GetLocationResponse>(location);

            return response;
        }
    }
}
