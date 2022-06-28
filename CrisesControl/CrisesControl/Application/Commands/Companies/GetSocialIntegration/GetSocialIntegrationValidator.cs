using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.GetSocialIntegration
{
    public class GetSocialIntegrationValidator:AbstractValidator<GetSocialIntegrationRequest>
    {
        public GetSocialIntegrationValidator()
        {
            RuleFor(a => a.CompanyID).GreaterThan(0);
            RuleFor(a => string.IsNullOrEmpty(a.AccountType)).ToString();
        }
    }
}
