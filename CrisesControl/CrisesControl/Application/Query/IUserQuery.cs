//using CrisesControl.Api.Application.Commands.Users.CreateUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.ViewModels.User;

namespace CrisesControl.Api.Application.Query
{
    public interface IUserQuery
    {
        public Task<GetUsersResponse> GetUsers(GetUsersRequest request);
        public Task<GetUserResponse> GetUser(GetUserRequest request);

    }
}
