using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.ForgotPassword
{
    public class ForgotPasswordValidator:AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(f => string.IsNullOrEmpty(f.EmailId));
        }
    }
}
