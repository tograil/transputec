using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;

namespace CrisesControl.Api.Application.Query {
    public interface IBillingQuery {
        Task<GetPaymentProfileResponse> GetPaymentProfile(GetPaymentProfileRequest request);
    }
}
