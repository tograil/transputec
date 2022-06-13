using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration
{
    public class VerifyTempRegistrationValidator:AbstractValidator<VerifyTempRegistrationRequest>
    {
        public VerifyTempRegistrationValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.UniqueRef));
        }
    }
}
