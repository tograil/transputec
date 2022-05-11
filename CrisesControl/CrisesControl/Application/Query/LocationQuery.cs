using AutoMapper;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocations;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;

namespace CrisesControl.Api.Application.Query
{
    public class LocationQuery: ILocationQuery
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        public LocationQuery(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<GetLocationsResponse> GetLocations(GetLocationsRequest request)
        {
            var locations = await _locationRepository.GetAllLocations(request.CompanyId);
            List<GetLocationResponse> response = _mapper.Map<List<Location>, List<GetLocationResponse>>(locations.ToList());
            var result = new GetLocationsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetLocationResponse> GetLocation(GetLocationRequest request)
        {
            var location = await _locationRepository.GetLocationById(request.LocationId);
            GetLocationResponse response = _mapper.Map<Location, GetLocationResponse>(location);

            return response;
        }
    }
}
