using FluentValidation;

namespace CrisesControl.Api.Application.Commands.SopLibrary.GetSopSection
{
    public class GetSopSectionValidator:AbstractValidator<GetSopSectionRequest>
    {
        public GetSopSectionValidator()
        {
            RuleFor(x => x.SOPHeaderID).GreaterThan(0);
        }
    }
}
