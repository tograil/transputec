using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetRegisteredUser
{
    public class GetRegisteredUserValidator : AbstractValidator<GetRegisteredUserRequest>
    {
        public GetRegisteredUserValidator()
        {
            RuleFor(x => x.QUserId)
                .GreaterThan(0);
        }
    }
}
