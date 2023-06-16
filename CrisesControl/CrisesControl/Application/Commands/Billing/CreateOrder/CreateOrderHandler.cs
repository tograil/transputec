using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.CreateOrder
{
    public class CreateOrderHandler:IRequestHandler<CreateOrderRequest, CreateOrderResponse>
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        public CreateOrderHandler(IBillingRepository billingRepository, IMapper mapper)
        {
            _billingRepository = billingRepository;
            _mapper = mapper;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateOrderRequest));

            var response = new CreateOrderResponse();
            var mappedRequest = _mapper.Map<CreateOrderRequest, OrderModel>(request);
            var result = await _billingRepository.CreateOrder(mappedRequest);
            response.OrderId = result;
            return response;
        }
    }
}
