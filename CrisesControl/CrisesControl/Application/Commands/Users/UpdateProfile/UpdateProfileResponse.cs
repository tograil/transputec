using System.Net;

namespace CrisesControl.Api.Application.Commands.Users.UpdateProfile
{
    public class UpdateProfileResponse
    {
        public int userId { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode ErrorCode { get; set; }
    }
}
