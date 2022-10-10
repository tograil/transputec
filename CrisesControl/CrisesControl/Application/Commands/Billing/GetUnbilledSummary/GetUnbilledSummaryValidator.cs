using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary
{
    public class GetUnbilledSummaryValidator:AbstractValidator<GetUnbilledSummaryRequest>
    {
        public GetUnbilledSummaryValidator()
        {
            RuleFor(e => e.MessageId).GreaterThan(0);
        }
    }
}
