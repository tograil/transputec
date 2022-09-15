using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Users.GetUserGroups
{
    public class GetUserGroupsResponse
    {
        public List<UserGroup> UserGroups { get; set; }
        public string Message { get; set; }
    }
}
