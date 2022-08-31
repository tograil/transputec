using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Payments.UpdateCompanyPaymentProfile
{
    public class UpdateCompanyPaymentProfileResponse
    {
        public CompanyPaymentProfile Data { get; set; }
        public string Message { get; set; }
    }
}
