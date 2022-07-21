using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetAllInvoices
{
    public class GetAllInvoicesHandler : IRequestHandler<GetAllInvoicesRequest, GetAllInvoicesResponse>
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;

        public GetAllInvoicesHandler(IBillingRepository billingRepository, IMapper mapper)
        {
            _billingRepository = billingRepository;
            _mapper = mapper;
        }
        public async Task<GetAllInvoicesResponse> Handle(GetAllInvoicesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllInvoicesRequest));

            var allInvoices = await _billingRepository.GetAllInvoices(request.CompanyId, cancellationToken);
            var result = _mapper.Map<GetAllInvoicesResponse>(allInvoices);
            return result;
        }
    }
}
