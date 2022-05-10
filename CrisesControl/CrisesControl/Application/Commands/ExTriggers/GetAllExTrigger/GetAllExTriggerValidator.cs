using FluentValidation;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetAllExTrigger
{
    public class GetAllExTriggerValidator:AbstractValidator<GetAllExTriggerRequest>
    {
        public GetAllExTriggerValidator()
        {
            RuleFor(x => x.CompanyID)
              .GreaterThan(0);
            RuleFor(x => x.UserID)
                .GreaterThan(0);
        }
    }
}
