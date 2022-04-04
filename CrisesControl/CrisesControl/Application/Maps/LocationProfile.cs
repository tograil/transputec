using AutoMapper;
using CrisesControl.Api.Application.Commands.Locations.CreateLocation;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Api.Application.Commands.Locations.UpdateLocation;
using CrisesControl.Core.LocationAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class LocationProfile : Profile
    {
        public LocationProfile() 
        {
            CreateMap<Location, GetLocationResponse>();

            CreateMap<CreateLocationRequest, Location>()
                .ForMember(x => x.CreatedOn, m => m.MapFrom(x => DateTimeOffset.Now))
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));

            CreateMap<UpdateLocationRequest, Location>()
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));
        }
    }
}
