using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.CheckEmail
{
    public class CheckEmailValidator: AbstractValidator<CheckEmailRequest>
    {
        public CheckEmailValidator()
        {
            RuleFor(x => x.EmailId)
                .NotEmpty();

        }
    }
}
