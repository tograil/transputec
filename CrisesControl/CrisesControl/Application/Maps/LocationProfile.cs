using AutoMapper;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Core.LocationAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class LocationProfile : Profile
    {
        public LocationProfile() 
        {
            CreateMap<Location, GetLocationResponse>();
        }
    }
}
