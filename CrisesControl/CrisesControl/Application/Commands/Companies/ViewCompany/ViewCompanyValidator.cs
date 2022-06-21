using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.ViewCompany
{
    public class ViewCompanyValidator:AbstractValidator<ViewCompanyRequest>
    {
        public ViewCompanyValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
        }
    }
}
