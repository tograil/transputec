using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Groups.SegregationLinks
{
    public class SegregationLinksValidator: AbstractValidator<SegregationLinksRequest>
    {
        public SegregationLinksValidator()
        {
            RuleFor(x => x.TargetID)
                .GreaterThan(0);
           

        }
    }
}
