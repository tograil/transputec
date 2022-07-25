using Ardalis.GuardClauses;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.CreateOrder
{
    public class CreateOrderHandler:IRequestHandler<CreateOrderRequest, CreateOrderResponse>
    {
        private readonly IBillingRepository _billingRepository;
        public CreateOrderHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateOrderRequest));

            var response = new CreateOrderResponse();
            response = _billingRepository.GetAllInvoices()
        }
    }
}
