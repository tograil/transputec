using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.UpdateProfile
{
    public class UpdateProfileValidator: AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            ;
        }
    }
}
