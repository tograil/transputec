using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserGroups
{
    public class GetUserGroupsRequest:IRequest<GetUserGroupsResponse>
    {
        public int UserId { get; set; }
    }
}
