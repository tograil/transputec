using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;
using Ardalis.GuardClauses;
using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvoicesById
{
    public class GetInvoicesByIdHandler:IRequestHandler<GetInvoicesByIdRequest,GetInvoicesByIdResponse>
    {
        private readonly GetInvoicesByIdValidator _getOrdersValidator;
        private readonly IBillingQuery _billingQuery;
        private readonly IMapper _mapper;
        private readonly ILogger<GetInvoicesByIdHandler> _logger;
        public GetInvoicesByIdHandler(GetInvoicesByIdValidator getOrdersValidator, ILogger<GetInvoicesByIdHandler> logger, IMapper mapper, IBillingQuery billingRepository)
        {
            _billingQuery = billingRepository;
            _mapper = mapper;
            _logger = logger;
            _getOrdersValidator = getOrdersValidator;
        }

        public async Task<GetInvoicesByIdResponse> Handle(GetInvoicesByIdRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetInvoicesByIdRequest));
            await _getOrdersValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _billingQuery.GetInvoicesById(request);
            return response;
        }
    }
}
