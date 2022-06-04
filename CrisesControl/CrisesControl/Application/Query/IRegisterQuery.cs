using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Api.Application.Commands.Register.ValidateMobile;
using CrisesControl.Api.Application.Commands.Register.ValidateUserEmail;

namespace CrisesControl.Api.Application.Query
{
    public interface IRegisterQuery
    {
        Task<CheckCustomerResponse> CheckCustomer(CheckCustomerRequest request);
        Task<VerifyPhoneResponse> ValidateMobile(VerifyPhoneRequest request);
        Task<ValidateUserEmailResponse> ValidateUserEmail(ValidateUserEmailRequest request);
    }
}
