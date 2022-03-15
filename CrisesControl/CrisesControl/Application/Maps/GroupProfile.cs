using AutoMapper;
using CrisesControl.Api.Application.Commands.Locations.GetLocation;
using CrisesControl.Core.GroupAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GetLocationResponse>();
        }
    }
}
