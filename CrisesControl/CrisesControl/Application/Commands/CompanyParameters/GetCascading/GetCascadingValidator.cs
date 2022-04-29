using FluentValidation;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading
{
    public class GetCascadingValidator: AbstractValidator<GetCascadingRequest>
    {
        public GetCascadingValidator()
        {
            RuleFor(x => x.CompanyID)
                .GreaterThan(0);
            RuleFor(x => x.PlanType != "" || !string.IsNullOrEmpty(x.PlanType));
        }
    }
}
