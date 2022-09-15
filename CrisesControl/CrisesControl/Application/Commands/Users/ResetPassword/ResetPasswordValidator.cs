using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.ResetPassword
{
    public class ResetPasswordValidator:AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {

        }
    }
}
