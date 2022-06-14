using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.CheckCompany
{
    public class CheckCompanyValidator:AbstractValidator<CheckCompanyRequest>
    {
        public CheckCompanyValidator()
        {
            RuleFor(x=>string.IsNullOrEmpty(x.CompanyName)).ToString();
            RuleFor(x => string.IsNullOrEmpty(x.CountryCode)).ToString();
        }
    }
}
