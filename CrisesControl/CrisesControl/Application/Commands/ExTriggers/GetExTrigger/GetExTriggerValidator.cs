using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger
{
    public class GetExTriggerValidator:AbstractValidator<GetExTriggerRequest>
    {
        public GetExTriggerValidator()
        {
            RuleFor(x => x.CompanyID)
                .GreaterThan(0);
            RuleFor(x => x.ExTriggerID)
                .GreaterThan(0);
        }
    }
}
