using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvoicesById
{
    public class GetInvoicesByIdHandler:IRequestHandler<GetInvoicesByIdRequest, GetInvoicesByIdResponse>
    {
        private readonly IBillingRepository _billingRepository;
        public GetInvoicesByIdHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<GetInvoicesByIdResponse> Handle(GetInvoicesByIdRequest request, CancellationToken cancellationToken)
        {
            var response = new GetInvoicesByIdResponse();
            var result = await _billingRepository.GetInvoicesById(request.CompanyId, request.TransactionHeaderId, request.ShowPayments);
            return result;
        }
    }
}
