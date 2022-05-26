using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserGroup
{
    public class UpdateUserGroupRequest:  IRequest<UpdateUserGroupResponse>
    {
        public List<UserGroup> UserGroups { get; set; }
    }
}
