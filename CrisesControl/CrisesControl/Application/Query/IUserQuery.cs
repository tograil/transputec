//using CrisesControl.Api.Application.Commands.Users.CreateUser;
using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUserComms;
using CrisesControl.Api.Application.Commands.Users.GetAllUser;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.ValidateEmail;
using CrisesControl.Api.Application.Commands.Users.MembershipList;
using CrisesControl.Api.Application.ViewModels.User;
using CrisesControl.Core.Users;
using CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList;
using CrisesControl.Api.Application.Commands.Users.ResetPassword;
using CrisesControl.Api.Application.Commands.Users.ForgotPassword;
using CrisesControl.Api.Application.Commands.Users.LinkResetPassword;

namespace CrisesControl.Api.Application.Query
{
    public interface IUserQuery
    {
        public Task<GetAllUserResponse> GetUsers(GetAllUserRequest request, CancellationToken cancellationToken);
        public Task<GetUserResponse> GetUser(GetUserRequest request, CancellationToken cancellationToken);
        public Task<LoginResponse> GetLoggedInUserInfo(LoginRequest request, CancellationToken cancellationToken);
        public Task<ActivateUserResponse> ReactivateUser(int queriedUserId, CancellationToken cancellationToken);
        public Task<GetAllUserDevicesResponse> GetAllUserDeviceList(GetAllUserDevicesRequest request, CancellationToken cancellationToken);

        Task<MembershipResponse> MembershipList(MembershipRequest request);

        public Task<ValidateEmailResponse> ValidateLoginEmail(ValidateEmailRequest request);
        public Task<IEnumerable<GetUserCommsResponse>> GetUserComms(GetUserCommsRequest request, CancellationToken cancellationToken);
        public Task<IEnumerable<GetAllOneUserDeviceListResponse>> GetAllOneUserDeviceList(GetAllOneUserDeviceListRequest request, CancellationToken cancellationToken);
        Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request);
        Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request);
        Task<LinkResetPasswordResponse> LinkResetPassword(LinkResetPasswordRequest request);
    }
}
