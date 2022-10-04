using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvoicesById
{
    public class GetInvoicesByIdValidator:AbstractValidator<GetInvoicesByIdRequest>
    {
        public GetInvoicesByIdValidator()
        {
            RuleFor(x => x.TransactionHeaderId).GreaterThan(0);
        }
    }
}
