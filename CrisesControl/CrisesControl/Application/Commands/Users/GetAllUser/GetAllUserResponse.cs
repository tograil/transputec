using CrisesControl.Api.Application.Commands.Users.GetUser;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUser
{
    public class GetAllUserResponse
    {
        public List<GetUserResponse> Data { get; set; }
    }
}
