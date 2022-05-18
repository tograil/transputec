using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.ActivateUser
{
    public class ActivateUserValidator:AbstractValidator<ActivateUserRequest>
    {
        public ActivateUserValidator()
        {
            RuleFor(x => x.QueriedUserId).NotEmpty();
        }
    }
}
