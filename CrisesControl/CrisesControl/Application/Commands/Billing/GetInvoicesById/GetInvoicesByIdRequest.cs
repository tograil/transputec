using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvoicesById
{
    public class GetInvoicesByIdRequest : IRequest<GetInvoicesByIdResponse>
    {
        public int TransactionHeaderId { get; set; }
        public bool ShowPayments { get; set; }
    }
}
