using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserPhone
{
    public class UpdateUserPhoneValidator: AbstractValidator<UpdateUserPhoneRequest>
    {
        public UpdateUserPhoneValidator()
        {
            RuleFor(x => x.MobileISDCode)
                .NotEmpty();
            RuleFor(x => x.MobileNo)
                .NotEmpty();

        }
    }
}
