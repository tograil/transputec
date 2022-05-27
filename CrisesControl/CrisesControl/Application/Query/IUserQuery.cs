//using CrisesControl.Api.Application.Commands.Users.CreateUser;
using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.ValidateEmail;
using CrisesControl.Api.Application.Commands.Users.MembershipList;
using CrisesControl.Api.Application.ViewModels.User;

namespace CrisesControl.Api.Application.Query
{
    public interface IUserQuery
    {
        public Task<GetUsersResponse> GetUsers(GetUsersRequest request, CancellationToken cancellationToken);
        public Task<GetUserResponse> GetUser(GetUserRequest request, CancellationToken cancellationToken);
        public Task<LoginResponse> GetLoggedInUserInfo(LoginRequest request, CancellationToken cancellationToken);
        public Task<ActivateUserResponse> ReactivateUser(int queriedUserId, CancellationToken cancellationToken);
        Task<MembershipResponse> MembershipList(MembershipRequest request);

        public Task<ValidateEmailResponse> ValidateLoginEmail(ValidateEmailRequest request);
    }
}
