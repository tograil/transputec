using CrisesControl.Api.Application.Commands.App.AppHome;
using CrisesControl.Api.Application.Commands.App.CaptureUserLocation;
using CrisesControl.Api.Application.Commands.App.CCPhoneNumbers;
using CrisesControl.Api.Application.Commands.App.CheckUserLocation;
using CrisesControl.Api.Application.Commands.App.GetAppLanguage;
using CrisesControl.Api.Application.Commands.App.GetLanguage;
using CrisesControl.Api.Application.Commands.App.GetPrivacyPolicy;
using CrisesControl.Api.Application.Commands.App.GetTnC;
using CrisesControl.Api.Application.Commands.App.ReferToFriend;
using CrisesControl.Api.Application.Commands.App.SendFeedback;
using CrisesControl.Api.Application.Commands.App.UpdateDevice;
using CrisesControl.Api.Application.Commands.App.UpdatePushToken;
using CrisesControl.Api.Application.Commands.App.UpdateTrackMe;
using CrisesControl.Api.Application.Commands.App.ValidatePin;

namespace CrisesControl.Api.Application.Query
{
    public interface IAppQuery
    {
        Task<GetTnCResponse> GetTnC(GetTnCRequest request);
        Task<GetPrivacyPolicyResponse> GetPrivacyPolicy(GetPrivacyPolicyRequest request);
        Task<ValidatePinResponse> ValidatePin(ValidatePinRequest request);
        Task<AppHomeResponse> AppHome(AppHomeRequest request);
        Task<SendFeedbackResponse> SendFeedback(SendFeedbackRequest request);
        Task<ReferToFriendResponse> ReferToFriend(ReferToFriendRequest request);
        Task<UpdateDeviceResponse> UpdateDevice(UpdateDeviceRequest request);
        Task<CCPhoneNumbersResponse> CCPhoneNumbers(CCPhoneNumbersRequest request);
        Task<CaptureUserLocationResponse> CaptureUserLocation(CaptureUserLocationRequest request);
        Task<CheckUserLocationResponse> CheckUserLocation(CheckUserLocationRequest request);
        Task<GetLanguageResponse> GetLanguage(GetLanguageRequest request);
        Task<UpdateTrackMeResponse> UpdateTrackMe(UpdateTrackMeRequest request);
        Task<GetAppLanguageResponse> GetAppLanguage(GetAppLanguageRequest request);
        Task<UpdatePushTokenResponse> UpdatePushToken(UpdatePushTokenRequest request);
    }
}
