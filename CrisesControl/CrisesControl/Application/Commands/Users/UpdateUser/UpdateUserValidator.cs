using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUser
{
    public class UpdateUserValidator: AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
