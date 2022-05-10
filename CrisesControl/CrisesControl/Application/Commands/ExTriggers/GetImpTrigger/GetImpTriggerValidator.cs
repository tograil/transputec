using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetImpTrigger
{
    public class GetImpTriggerValidator:AbstractValidator<GetImpTriggerRequest>
    {
        public GetImpTriggerValidator()
        {
            RuleFor(x => x.CompanyID)
                .GreaterThan(0);
            RuleFor(x => x.UserID)
                .GreaterThan(0);

        }
    }
}
