using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.GetSite
{
    public class GetSiteValidator:AbstractValidator<GetSiteRequest>
    {
        public GetSiteValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
            RuleFor(x => x.SiteId).GreaterThan(0);
        }
    }
}
