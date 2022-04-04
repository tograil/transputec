using AutoMapper;
using CrisesControl.Api.Application.Commands.Groups.CreateGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.UpdateGroup;
using CrisesControl.Core.GroupAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GetGroupResponse>();


            CreateMap<CreateGroupRequest, Group>()
                .ForMember(x => x.CreatedOn, m => m.MapFrom(x => DateTimeOffset.Now))
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));

            CreateMap<UpdateGroupRequest, Group>()
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));
        }
    }
}
