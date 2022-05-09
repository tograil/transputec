using AutoMapper;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Maps {
    public class BillingProfile : Profile {
        public BillingProfile() {
            
            CreateMap<GetPaymentProfileRequest, BillingPaymentProfile>();
            CreateMap<BillingPaymentProfile, GetPaymentProfileResponse>();
        }
    }
}
