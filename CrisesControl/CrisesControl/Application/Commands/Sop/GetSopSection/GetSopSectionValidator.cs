using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Sop.GetSopSection
{
    public class GetSopSectionValidator:AbstractValidator<GetSopSectionRequest>
    {
        public GetSopSectionValidator()
        {
            RuleFor(x => x.ContentSectionID).GreaterThan(0);
            RuleFor(x => x.SOPHeaderID).GreaterThan(0);
        }
    }
}
