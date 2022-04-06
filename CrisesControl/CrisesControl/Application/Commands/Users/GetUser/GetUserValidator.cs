using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUser
{
    public class GetUserValidator: AbstractValidator<GetUserRequest>
    {
        public GetUserValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
