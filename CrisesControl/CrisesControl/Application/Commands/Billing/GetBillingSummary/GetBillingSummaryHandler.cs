using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetBillingSummary
{
    public class GetBillingSummaryHandler : IRequestHandler<GetBillingSummaryRequest, GetBillingSummaryResponse>
    {

        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;

        public GetBillingSummaryHandler(IBillingRepository billingRepository, IMapper mapper)
        {
            _billingRepository = billingRepository;
            _mapper = mapper;
        }

        public async Task<GetBillingSummaryResponse> Handle(GetBillingSummaryRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetBillingSummaryRequest));

            var billingSummary = await _billingRepository.GetBillingSummary(request.OutUserCompanyId, request.ChkUserId);
            var result = _mapper.Map<GetBillingSummaryResponse>(billingSummary);
            return result;
        }
    }
}
