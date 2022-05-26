using System.Net;

namespace CrisesControl.Api.Application.Commands.Users.UpdateGroupMember
{
    public class UpdateGroupMemberResponse
    {
        public bool result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
