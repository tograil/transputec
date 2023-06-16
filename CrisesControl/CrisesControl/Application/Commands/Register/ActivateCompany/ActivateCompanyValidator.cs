using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.ActivateCompany
{
    public class ActivateCompanyValidator : AbstractValidator<ActivateCompanyRequest>
    {
        public ActivateCompanyValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => string.IsNullOrEmpty(x.ActivationKey));
        }
    }
}
