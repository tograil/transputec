using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvoicesById
{
    public class GetInvoicesByIdRequest : IRequest<GetInvoicesByIdResponse>
    {
        public int CompanyId { get; set; }
        public int TransactionHeaderId { get; set; }
        public bool ShowPayments { get; set; }
    }
}
