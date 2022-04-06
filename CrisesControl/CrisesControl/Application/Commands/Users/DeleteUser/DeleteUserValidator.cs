using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.DeleteUser
{
    public class DeleteUserValidator: AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
