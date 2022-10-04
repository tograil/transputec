using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetAllInvoices
{
    public class GetAllInvoicesHandler : IRequestHandler<GetAllInvoicesRequest, GetAllInvoicesResponse>
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public GetAllInvoicesHandler(IBillingRepository billingRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _billingRepository = billingRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }
        public async Task<GetAllInvoicesResponse> Handle(GetAllInvoicesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllInvoicesRequest));

            var allInvoices = await _billingRepository.GetAllInvoices(_currentUser.CompanyId);
            var result = _mapper.Map<GetCompanyInvoicesReturn>(allInvoices);
            var response = new GetAllInvoicesResponse();
            response.AllInvoices = result;
            return response;
        }
    }
}
