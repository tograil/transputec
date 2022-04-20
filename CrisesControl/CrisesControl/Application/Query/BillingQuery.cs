using AutoMapper;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;

namespace CrisesControl.Api.Application.Query {
    public class BillingQuery : IBillingQuery  {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;

        public BillingQuery(IBillingRepository billingRepository, IMapper mapper) {
            _mapper = mapper;
            _billingRepository = billingRepository;
        }

        public async Task<GetPaymentProfileResponse> GetPaymentProfile(GetPaymentProfileRequest request) {
            var p_profile = await _billingRepository.GetPaymentProfile(request.CompanyId);
            GetPaymentProfileResponse result = _mapper.Map<BillingPaymentProfile, GetPaymentProfileResponse>(p_profile);
            return result;
        }
    }
}
