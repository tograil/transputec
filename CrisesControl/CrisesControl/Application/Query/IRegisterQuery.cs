using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Api.Application.Commands.Register.CreateSampleIncident;
using CrisesControl.Api.Application.Commands.Register.DeleteTempRegistration;
using CrisesControl.Api.Application.Commands.Register.GetTempRegistration;
using CrisesControl.Api.Application.Commands.Register.SetupCompleted;
using CrisesControl.Api.Application.Commands.Register.TempRegister;
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
        Task<TempRegisterResponse> TempRegister(TempRegisterRequest request);
        Task<SetupCompletedResponse> SetupCompleted(SetupCompletedRequest request);
        Task<GetTempRegistrationReponse> GetTempRegistration(GetTempRegistrationRequest request);
        Task<DeleteTempRegistrationResponse> DeleteTempRegistration(DeleteTempRegistrationRequest request);
        Task<CreateSampleIncidentResponse> CreateSampleIncident(CreateSampleIncidentRequest request);
    }
}
