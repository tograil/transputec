using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.ReactivateCompany
{
    public class ReactivateCompanyValidator:AbstractValidator<ReactivateCompanyRequest>
    {
        public ReactivateCompanyValidator()
        {
            RuleFor(x => x.ActivateReactivateCompanyId).GreaterThan(0);
        }
    }
}
