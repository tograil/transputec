using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;
using FluentValidation;
using Ardalis.GuardClauses;
namespace CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary
{
    public class GetUnbilledSummaryHandler:IRequestHandler<GetUnbilledSummaryRequest, GetUnbilledSummaryResponse>
    {
        private readonly GetUnbilledSummaryValidator _getUnbilledSummaryValidator;
        private readonly IBillingQuery _billingQuery;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUnbilledSummaryHandler> _logger;
        public GetUnbilledSummaryHandler(GetUnbilledSummaryValidator getUnbilledSummaryValidator, ILogger<GetUnbilledSummaryHandler> logger, IMapper mapper, IBillingQuery billingRepository)
        {
            _billingQuery = billingRepository;
            _mapper = mapper;
            _logger = logger;
            _getUnbilledSummaryValidator = getUnbilledSummaryValidator;
        }

        public async Task<GetUnbilledSummaryResponse> Handle(GetUnbilledSummaryRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUnbilledSummaryRequest));
            await _getUnbilledSummaryValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _billingQuery.GetUnbilledSummary(request);
            return response;
        }
    }
}
