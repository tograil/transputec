using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails
{
    public class GetTransactionDetailsValidator:AbstractValidator<GetTransactionDetailsRequest>
    {
        public GetTransactionDetailsValidator()
        {
            RuleFor(e => e.messageId).GreaterThan(0);
        }
    }
}
