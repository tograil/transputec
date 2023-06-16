using System.Net;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserGroup
{
    public class UpdateUserGroupResponse
    {
        public bool result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
