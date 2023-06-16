using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;
using FluentValidation;
using Ardalis.GuardClauses;

namespace CrisesControl.Api.Application.Commands.Billing.GetUsageGraph
{
    public class GetUsageGraphHandler:IRequestHandler<GetUsageGraphRequest, GetUsageGraphResponse>
    {
        private readonly GetUsageGraphValidator _getUsageGraphValidator;
        private readonly IBillingQuery _billingQuery;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsageGraphHandler> _logger;
        public GetUsageGraphHandler(GetUsageGraphValidator getUsageGraphValidator, ILogger<GetUsageGraphHandler> logger, IMapper mapper, IBillingQuery billingRepository)
        {
            _billingQuery = billingRepository;
            _mapper = mapper;
            _logger = logger;
            _getUsageGraphValidator = getUsageGraphValidator;
        }

        public async Task<GetUsageGraphResponse> Handle(GetUsageGraphRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUsageGraphRequest));
            await _getUsageGraphValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _billingQuery.GetUsageGraph(request);
            return response;
        }
    }
}
