using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Locations.DeleteLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocations;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using FluentValidation;

namespace CrisesControl.Api.Application.Query
{
    public class LocationQuery: ILocationQuery
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;


        private readonly ICurrentUser _currentUser;
        public LocationQuery(ILocationRepository locationRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<GetLocationsResponse> GetLocations(GetLocationsRequest request, CancellationToken cancellationToken)
        {
            var locations = await _locationRepository.GetAllLocations(request.CompanyId, request.FilterVirtual);
            var result = _mapper.Map<List<LocationDetail>>(locations);
            var response = new GetLocationsResponse();
            response.Data = result;
            return response;
        }

        public async Task<GetLocationResponse> GetLocation(GetLocationRequest request, CancellationToken cancellationToken)
        {

            var response = new GetLocationResponse();
            var location = await _locationRepository.GetLocationById(request.LocationId);
            if (location != null) {

                response.CompanyId = location.CompanyId;
                response.Desc = location.Desc;
                response.CreatedBy = location.CreatedBy;
                response.Lat = location.Lat;
                response.CreatedOn = location.CreatedOn;
                response.LocationName = location.LocationName;
                response.Long = location.Long;
                response.PostCode = location.PostCode;
                response.Status = location.Status;
                response.UpdatedBy = location.UpdatedBy;
                response.UpdatedOn = location.UpdatedOn;
                response.LocationId = location.LocationId;
             }
            return response;
        }

        public async Task<DeleteLocationResponse> DeleteLocation(DeleteLocationRequest request)
        {
            try
            {
                var location = await _locationRepository.DeleteLocation(request.LocationId,_currentUser.CompanyId,_currentUser.UserId);
                //var result = _mapper.Map<bool>(location);
                var response = new DeleteLocationResponse();
                if (location)
                {
                    response.Message = "Deleted";
                    response.Result = true;
                }
                else
                {
                    response.Message = "Not Data Found";
                    response.Result = false;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
