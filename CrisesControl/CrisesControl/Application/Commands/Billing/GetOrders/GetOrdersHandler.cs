using Ardalis.GuardClauses;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetOrders
{
    public class GetOrdersHandler: IRequestHandler<GetOrdersRequest,GetOrdersResponse>
    {
        private readonly IBillingRepository _billingRepository;
        public GetOrdersHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<GetOrdersResponse> Handle(GetOrdersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetOrdersRequest));

            var orderList = await _billingRepository.GetOrder(request.OrderId, request.CompanyId,request.CustomerId, request.OriginalOrderId);
            var response = new GetOrdersResponse();
            response.Response = orderList;
            return response;
        }
    }
}
