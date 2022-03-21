using AutoMapper;
using CrisesControl.Api.Application.Commands.Groups.CreateGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Core.GroupAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GetGroupResponse>();

            CreateMap<Group, CreateGroupRequest>();
        }
    }
}
