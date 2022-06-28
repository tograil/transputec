using FluentValidation;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.DeleteCascading
{
    public class DeleteCascadingValidator:AbstractValidator<DeleteCascadingRequest>
    {
        public DeleteCascadingValidator()
        {
            RuleFor(x=>x.CompanyId).GreaterThan(0);
            RuleFor(x => x.PlanID).GreaterThan(0);
        }
    }
}
