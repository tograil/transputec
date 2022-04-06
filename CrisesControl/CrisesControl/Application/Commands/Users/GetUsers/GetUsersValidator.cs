using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUsers
{
    public class GetUsersValidator: AbstractValidator<GetUsersRequest>
    {
        public GetUsersValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
