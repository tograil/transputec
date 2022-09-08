using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.LinkResetPassword
{
    public class LinkResetPasswordValidator:AbstractValidator<LinkResetPasswordRequest>
    {
        public LinkResetPasswordValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.NewPassword));
            RuleFor(x => string.IsNullOrEmpty(x.QueriedGuid));
        }
    }
}
