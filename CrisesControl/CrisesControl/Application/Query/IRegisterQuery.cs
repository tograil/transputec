using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Api.Application.Commands.Register.UpgradeRequest;
using CrisesControl.Api.Application.Commands.Register.ValidateMobile;
using CrisesControl.Api.Application.Commands.Register.ValidateUserEmail;
using CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration;

namespace CrisesControl.Api.Application.Query
{
    public interface IRegisterQuery
    {
        Task<CheckCustomerResponse> CheckCustomer(CheckCustomerRequest request);
        Task<VerifyPhoneResponse> ValidateMobile(VerifyPhoneRequest request);
        Task<ValidateUserEmailResponse> ValidateUserEmail(ValidateUserEmailRequest request);
        Task<UpgradeResponse> UpgradeRequest(UpgradeRequest request);
        Task<VerifyTempRegistrationResponse> VerifyTempRegistration(VerifyTempRegistrationRequest request);
    }
}
