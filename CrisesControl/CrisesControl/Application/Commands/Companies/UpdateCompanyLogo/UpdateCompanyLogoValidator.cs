using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyLogo
{
    public class UpdateCompanyLogoValidator : AbstractValidator<UpdateCompanyLogoRequest>
    {
        public UpdateCompanyLogoValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.CompanyLogo));
             
        }
    }
}
