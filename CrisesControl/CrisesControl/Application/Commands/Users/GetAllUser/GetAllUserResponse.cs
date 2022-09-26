using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUser
{
    public class GetAllUserResponse
    {
        public List<User> Data { get; set; }
    }
}
