using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;
using FluentValidation;
using Ardalis.GuardClauses;

namespace CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails
{
    public class GetTransactionDetailsHandler:IRequestHandler<GetTransactionDetailsRequest, GetTransactionDetailsResponse>
    {
        private readonly GetTransactionDetailsValidator _getTransactionDetailsValidator;
        private readonly IBillingQuery _billingQuery;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionDetailsHandler> _logger;
        public GetTransactionDetailsHandler(GetTransactionDetailsValidator getTransactionDetailsValidator, ILogger<GetTransactionDetailsHandler> logger, IMapper mapper, IBillingQuery billingRepository)
        {
            _billingQuery = billingRepository;
            _mapper = mapper;
            _logger = logger;
            _getTransactionDetailsValidator = getTransactionDetailsValidator;
        }

        public async Task<GetTransactionDetailsResponse> Handle(GetTransactionDetailsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTransactionDetailsRequest));
            await _getTransactionDetailsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _billingQuery.GetTransactionDetails(request);
            return response;
        }
    }
}
