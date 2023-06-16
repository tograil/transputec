using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.SegregationLinks
{
    public class SegregationLinksValidator:AbstractValidator<SegregationLinksRequest>
    {
        public SegregationLinksValidator()
        {
            RuleFor(x => x.TargetID).GreaterThan(0);
            RuleFor(x => x.OutUserCompanyId).GreaterThan(0);
            RuleFor(x => string.IsNullOrEmpty(x.MemberShipType));
            RuleFor(x => string.IsNullOrEmpty(x.LinkType));
        }
    }
}
