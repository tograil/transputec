using CrisesControl.Api.Application.Commands.Communication.DeleteRecording;
using CrisesControl.Api.Application.Commands.Communication.DownloadRecording;
using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences;
using CrisesControl.Api.Application.Commands.Communication.HandelCallResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelCMSMSResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelConfRecording;
using CrisesControl.Api.Application.Commands.Communication.HandelConfResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelPushResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelSMSResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelTwoFactor;
using CrisesControl.Api.Application.Commands.Communication.HandelUnifonicCallResponse;
using CrisesControl.Api.Application.Commands.Communication.StartConference;
using CrisesControl.Api.Application.Commands.Communication.TwilioCall;
using CrisesControl.Api.Application.Commands.Communication.TwilioCallAck;
using CrisesControl.Api.Application.Commands.Communication.TwilioCallLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioConfLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioEndConferenceCall;
using CrisesControl.Api.Application.Commands.Communication.TwilioRecordingLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioText;
using CrisesControl.Api.Application.Commands.Communication.TwilioTextLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioVerify;
using CrisesControl.Api.Application.Commands.Communication.TwilioVerifyCheck;

namespace CrisesControl.Api.Application.Query {
    public interface ICommunicationQuery {
        public Task<GetUserActiveConferencesResponse> GetUserActiveConferences(GetUserActiveConferencesRequest request);
        Task<HandelCallResponseResponse> HandelCallResponse(HandelCallResponseRequest request);
        Task<HandelUnifonicCallResponse> HandelUnifonicCallResponse(HandelUnifonicCallResponseRequest request);
        Task<TwilioCallAckResponse> TwilioCallAck(TwilioCallAckRequest request);
        Task<HandelSMSResponse> HandelSMSResponse(HandelSMSResponseRequest request);
        Task<HandelCMSMSResponse> HandelCMSMSResponse(HandelCMSMSResponseRequest request);
        Task<HandelPushResponse> HandelPushResponse(HandelPushResponseRequest request);
        Task<HandelConfResponse> HandelConfResponse(HandelConfResponseRequest request);
        Task<HandelConfRecordingResponse> HandelConfRecording(HandelConfRecordingRequest request);
        Task<StartConferenceResponse> StartConference(StartConferenceRequest request);
        Task<DownloadRecordingResponse> DownloadRecording(DownloadRecordingRequest request);
        Task<TwilioTextResponse> TwilioText(TwilioTextRequest request);
        Task<TwilioCallResponse> TwilioCall(TwilioCallRequest request);
        Task<DeleteRecordingResponse> DeleteRecording(DeleteRecordingRequest request);
        Task<TwilioTextLogResponse> TwilioTextLog(TwilioTextLogRequest request);
        Task<TwilioCallLogResponse> TwilioCallLog(TwilioCallLogRequest request);
        Task<TwilioConfLogResponse> TwilioConfLog(TwilioConfLogRequest request);
        Task<TwilioRecordingLogResponse> TwilioRecordingLog(TwilioRecordingLogRequest request);
        Task<HandelTwoFactorResponse> HandelTwoFactor(HandelTwoFactorRequest request);
        Task<TwilioVerifyResponse> TwilioVerify(TwilioVerifyRequest request);
        Task<TwilioVerifyCheckResponse> TwilioVerifyCheck(TwilioVerifyCheckRequest request);
        Task<TwilioEndConferenceCallResponse> TwilioEndConferenceCall(TwilioEndConferenceCallRequest request);
    }
}
