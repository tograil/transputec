using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile {
    public class GetPaymentProfileResponse {
        public string PaidServices { get; set; }
        public CompanyPaymentProfile Profile { get; set; }
    }
}
