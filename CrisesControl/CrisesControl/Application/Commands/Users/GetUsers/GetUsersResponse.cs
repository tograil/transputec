using CrisesControl.Api.Application.Commands.Users.GetUser;

namespace CrisesControl.Api.Application.Commands.Users.GetUsers
{
    public class GetUsersResponse
    {
        public List<GetUserResponse> Data { get; set; }
    }
}
