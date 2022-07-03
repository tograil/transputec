using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.ReactivateUser
{
    public class ReactivateUserValidator : AbstractValidator<ReactivateUserRequest>
    {
        public ReactivateUserValidator()
        {
            RuleFor(x => x.QueriedUserId)
                .NotEmpty();
            RuleFor(x => x.Filters)
                .NotEmpty();
        }
    }
}
