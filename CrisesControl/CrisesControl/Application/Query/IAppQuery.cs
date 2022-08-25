using CrisesControl.Api.Application.Commands.App.GetPrivacyPolicy;
using CrisesControl.Api.Application.Commands.App.GetTnC;
using CrisesControl.Api.Application.Commands.App.ValidatePin;

namespace CrisesControl.Api.Application.Query
{
    public interface IAppQuery
    {
        Task<GetTnCResponse> GetTnC(GetTnCRequest request);
        Task<GetPrivacyPolicyResponse> GetPrivacyPolicy(GetPrivacyPolicyRequest request);
        Task<ValidatePinResponse> ValidatePin(ValidatePinRequest request);
    }
}
