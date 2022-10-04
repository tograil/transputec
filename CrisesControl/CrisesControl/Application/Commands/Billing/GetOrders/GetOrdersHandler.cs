using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;
using FluentValidation;
using Ardalis.GuardClauses;

namespace CrisesControl.Api.Application.Commands.Billing.GetOrders
{
    public class GetOrdersHandler : IRequestHandler<GetOrdersRequest, GetOrdersResponse>
    {
        private readonly GetOrdersValidator _getOrdersValidator;
        private readonly IBillingQuery _billingQuery;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrdersHandler> _logger;
        public GetOrdersHandler(GetOrdersValidator getOrdersValidator, ILogger<GetOrdersHandler> logger, IMapper mapper, IBillingQuery billingRepository)
        {
            _billingQuery = billingRepository;
            _mapper = mapper;
            _logger = logger;
            _getOrdersValidator = getOrdersValidator;
        }
        public async Task<GetOrdersResponse> Handle(GetOrdersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetOrdersRequest));
            await _getOrdersValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _billingQuery.GetOrders(request);
            return response;
        }
    }
}
