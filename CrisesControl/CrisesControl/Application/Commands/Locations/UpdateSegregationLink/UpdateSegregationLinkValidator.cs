using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateSegregationLink
{
    public class UpdateSegregationLinkValidator : AbstractValidator<UpdateSegregationLinkRequest>
    {
        public UpdateSegregationLinkValidator()
        {
            RuleFor(x => x.TargetID).GreaterThan(0);
            RuleFor(x => x.SourceID).GreaterThan(0);
        }
    }
}
