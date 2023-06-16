using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.DeactivateCompany
{
    public class DeactivateCompanyValidator:AbstractValidator<DeactivateCompanyRequest>
    {
        public DeactivateCompanyValidator()
        {
            RuleFor(x => x.TargetCompanyID).GreaterThan(0);
        }
    }
}
