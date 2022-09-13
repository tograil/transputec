using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.Users.GetUserId
{
    public class GetUserIdResponse
    {
        public User User { get; set; }
        public string Message { get; set; }
    }
}
