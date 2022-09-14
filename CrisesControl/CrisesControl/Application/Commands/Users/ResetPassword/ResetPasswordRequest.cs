using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ResetPassword
{
    public class ResetPasswordRequest:IRequest<ResetPasswordResponse>
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
