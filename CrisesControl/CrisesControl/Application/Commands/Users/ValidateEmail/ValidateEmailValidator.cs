using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.ValidateEmail
{
    public class ValidateEmailValidator: AbstractValidator<ValidateEmailRequest>
    {
        public ValidateEmailValidator()
        {
            RuleFor(x => x.UserEmail)
                .NotNull()
                .NotEmpty();
        }
    }
}
