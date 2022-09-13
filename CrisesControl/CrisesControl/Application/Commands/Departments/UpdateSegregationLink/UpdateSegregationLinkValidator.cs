using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink
{
    public class UpdateSegregationLinkValidator:AbstractValidator<UpdateSegregationLinkRequest>
    {
        public UpdateSegregationLinkValidator()
        {
            RuleFor(x => x.TargetId).GreaterThan(0);
            RuleFor(x => x.SourceId).GreaterThan(0);
        }
    }
}
