using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.AddUser
{
    public class AddUserValidator: AbstractValidator<AddUserRequest>
    {
        public AddUserValidator()
        {
               RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
