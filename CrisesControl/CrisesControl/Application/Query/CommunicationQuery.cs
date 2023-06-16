using AutoMapper;
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
using CrisesControl.Core.Communication;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Register;
using CrisesControl.SharedKernel.Utils;
using Twilio.Base;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

namespace CrisesControl.Api.Application.Query {
    public class CommunicationQuery : ICommunicationQuery {
        private readonly ICommunicationRepository _communicationRepository;
        private readonly IMapper _mapper;

        public CommunicationQuery(ICommunicationRepository communicationRepository, IMapper mapper,
           ILogger<BillingQuery> logger) {
            _mapper = mapper;
            _communicationRepository = communicationRepository;
        }

        public async Task<DeleteRecordingResponse> DeleteRecording(DeleteRecordingRequest request)
        {
            try
            {
                dynamic CommsAPI = _communicationRepository.InitComms("TWILIO", "", request.ClientId, request.Secret, request.DataCenter);
                CommsAPI.SendInDirect = false;
                CommsAPI.DeleteRecording(request.RecordingSid);
                var result = _mapper.Map<bool>(CommsAPI);
                var response = new DeleteRecordingResponse();
                if (result!=null)
                {
                    response.Result = true;
                   
                }
                else
                {

                    response.Result = false;
                   
                }
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<DownloadRecordingResponse> DownloadRecording(DownloadRecordingRequest request)
        {
            try
            {
                var recoding =await  _communicationRepository.DownloadRecording(request.FileName);
                
                var result = _mapper.Map<HttpResponseMessage>(recoding);
                var response = new DownloadRecordingResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = new HttpResponseMessage();

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<GetUserActiveConferencesResponse> GetUserActiveConferences(GetUserActiveConferencesRequest request) {
            var conflist = await _communicationRepository.GetUserActiveConferences();
            var response = _mapper.Map<List<UserConferenceItem>>(conflist);
            var result = new GetUserActiveConferencesResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<HandelCallResponseResponse> HandelCallResponse(HandelCallResponseRequest request)
        {
            try
            {
                var recoding = await _communicationRepository.HandelCallResponse(request.CallSid, request.CallStatus, request.From, request.To, request.CallDuration, "TWILIO");

                var result = _mapper.Map<string>(recoding);
                var response = new HandelCallResponseResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<HandelCMSMSResponse> HandelCMSMSResponse(HandelCMSMSResponseRequest request)
        {
            try
            {
                var smsReponse = await _communicationRepository.HandelSMSResponse(request.MessageSid, request.Status, "", request.To, request.Body, "CM");

                var result = _mapper.Map<string>(smsReponse);
                var response = new HandelCMSMSResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<HandelConfRecordingResponse> HandelConfRecording(HandelConfRecordingRequest request)
        {
            try
            {
                var smsReponse = await _communicationRepository.HandelConfRecording(request.ConferenceSid, request.RecordingSid, request.RecordingUrl, request.RecordingStatus,
                    request.RecordingFileSize, request.Duration);

                var result = _mapper.Map<string>(smsReponse);
                var response = new HandelConfRecordingResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<HandelConfResponse> HandelConfResponse(HandelConfResponseRequest request)
        {
            try
            {
                var confReponse = await _communicationRepository.HandelConfResponse(request.CallSid, request.ConferenceSid, request.StatusCallbackEvent);

                var result = _mapper.Map<string>(confReponse);
                var response = new HandelConfResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<HandelPushResponse> HandelPushResponse(HandelPushResponseRequest request)
        {
            try
            {
                var confReponse = await _communicationRepository.HandelPushResponse(request.SendBackId);

                var result = _mapper.Map<string>(confReponse);
                var response = new HandelPushResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<HandelSMSResponse> HandelSMSResponse(HandelSMSResponseRequest request)
        {
            try
            {
                var smsResponse = await _communicationRepository.HandelTwoFactor(request.MessageSid, request.SmsStatus);

                var result = _mapper.Map<string>(smsResponse);
                var response = new HandelSMSResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<HandelTwoFactorResponse> HandelTwoFactor(HandelTwoFactorRequest request)
        {
            try
            {
                var smsResponse = await _communicationRepository.HandelTwoFactor(request.MessageSid, request.SmsStatus);

                var result = _mapper.Map<string>(smsResponse);
                var response = new HandelTwoFactorResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<HandelUnifonicCallResponse> HandelUnifonicCallResponse(HandelUnifonicCallResponseRequest request)
        {
            try
            {
                var smsResponse = await _communicationRepository.HandelUnifonicCallResponse(request.referenceId, request.callSid, request.status, request.from, request.to, request.timestamp, request.duration, "UNIFONIC"); ;

                var result = _mapper.Map<string>(smsResponse);
                var response = new HandelUnifonicCallResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<StartConferenceResponse> StartConference(StartConferenceRequest request)
        {
            try
            {
                var smsResponse = await _communicationRepository.StartConferenceNew(request.CompanyId, request.UserId, request.ConfUsers, "GMT Standard Time",
                    request.ActiveIncidentId, request.IncidentMessageId, "Incident");
                var result = _mapper.Map<string>(smsResponse);
                var response = new StartConferenceResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {

                    response.Message = "No record found.";

                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioCallResponse> TwilioCall(TwilioCallRequest request)
        {
            try
            {
                var twillioCall = await _communicationRepository.TwilioCall(request.Model);
                var result = _mapper.Map<CommsStatus>(twillioCall);
                var response = new TwilioCallResponse();
                if (result != null)
                {
                    response.Data = result;

                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioCallAckResponse> TwilioCallAck(TwilioCallAckRequest request)
        {
            try
            {
                var twillioCall = await _communicationRepository.TwilioCallAck(request.context);
                var result = _mapper.Map<string>(twillioCall);
                var response = new TwilioCallAckResponse();
                if (result != null)
                {
                    response.Message = result;

                }
                else
                {
                    response.Message = "No record Found.";
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioCallLogResponse> TwilioCallLog(TwilioCallLogRequest request)
        {
            try
            {
                var twillioCall = await _communicationRepository.TwilioCallLog(request.Model);
                var result = _mapper.Map<CallResource>(twillioCall);
                var response = new TwilioCallLogResponse();
                if (result != null)
                {
                    response.Data = result;

                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioConfLogResponse> TwilioConfLog(TwilioConfLogRequest request)
        {
            try
            {
                var twillioCall = await _communicationRepository.TwilioConfLog(request.Model);
                var result = _mapper.Map<ConferenceResource>(twillioCall);
                var response = new TwilioConfLogResponse();
                if (result != null)
                {
                    response.Data = result;

                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioEndConferenceCallResponse> TwilioEndConferenceCall(TwilioEndConferenceCallRequest request)
        {
            try
            {
                dynamic CommsAPI = await  _communicationRepository.InitComms("TWILIO", "", request.ClientId, request.Secret, request.DataCenter);
                CommsAPI.SendInDirect = false;
                var result = CommsAPI.EndConferenceCall(request.Sid);
               var response = new TwilioEndConferenceCallResponse();
                if (result != null)
                {
                    response.Data = result;

                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioRecordingLogResponse> TwilioRecordingLog(TwilioRecordingLogRequest request)
        {
            try
            {
                var twillioRecord = await _communicationRepository.TwilioRecordingLog(request.Model);
                var result = _mapper.Map<RecordingResource>(twillioRecord);
                var response = new TwilioRecordingLogResponse();
                if (result != null)
                {
                    response.Data = result;
                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioTextResponse> TwilioText(TwilioTextRequest request)
        {
            try
            {
                var recordingLog = await _communicationRepository.TwilioRecordingLog(request.Model);
                var result = _mapper.Map<ResourceSet<RecordingResource>>(recordingLog);
                var response = new TwilioTextResponse();
                if (result != null)
                {
                    response.Data = result;
                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioTextLogResponse> TwilioTextLog(TwilioTextLogRequest request)
        {
            try
            {
                var textLog = await _communicationRepository.TwilioTextLog(request.Model);
                var result = _mapper.Map<MessageResource>(textLog);
                var response = new TwilioTextLogResponse();
                if (result != null)
                {
                    response.Data = result;
                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioVerifyResponse> TwilioVerify(TwilioVerifyRequest request)
        {
            try
            {
                var textLog = await _communicationRepository.TwilioVerify(request.Model);
                var result = _mapper.Map<VerificationResource>(textLog);
                var response = new TwilioVerifyResponse();
                if (result != null)
                {
                    response.Data = result;
                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        public async Task<TwilioVerifyCheckResponse> TwilioVerifyCheck(TwilioVerifyCheckRequest request)
        {
            try
            {
                var textLog = await _communicationRepository.TwilioVerifyCheck(request.Model);
                var result = _mapper.Map<VerificationCheckResource>(textLog);
                var response = new TwilioVerifyCheckResponse();
                if (result != null)
                {
                    response.Data = result;
                }
                else
                {
                    response.Data = null;
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }
    }
}
