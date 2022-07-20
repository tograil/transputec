using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetAllInvoices
{
    public class GetAllInvoicesRequest: IRequest<GetAllInvoicesResponse>
    {
        public int CompanyId { get; set; }
    }
}
