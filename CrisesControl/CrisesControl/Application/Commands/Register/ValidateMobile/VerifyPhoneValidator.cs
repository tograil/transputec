using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.ValidateMobile
{
    public class VerifyPhoneValidator : AbstractValidator<VerifyPhoneRequest>
    {
        public VerifyPhoneValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.MobileNo));
        }
    }
}
