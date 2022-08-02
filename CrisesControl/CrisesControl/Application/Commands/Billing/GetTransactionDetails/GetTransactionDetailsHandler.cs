using Ardalis.GuardClauses;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails
{
    public class GetTransactionDetailsHandler : IRequestHandler<GetTransactionDetailsRequest, GetTransactionDetailsResponse>
    {
        private readonly IBillingRepository _billingRepository;
        public GetTransactionDetailsHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<GetTransactionDetailsResponse> Handle(GetTransactionDetailsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTransactionDetailsRequest));

            var response = new GetTransactionDetailsResponse();
            response.Result = _billingRepository.GetTransactionItem(request.companyId, request.messageId, request.method, request.recordStart, request.recordLength, request.searchString, request.orderBy, request.orderDir, request.companyKey);
            return response;
        }
    }
}
