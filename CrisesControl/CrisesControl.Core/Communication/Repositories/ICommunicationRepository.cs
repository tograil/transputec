using CrisesControl.Core.Register;
using CrisesControl.Core.Users;
using CrisesControl.SharedKernel.Enums;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Twilio.Base;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

namespace CrisesControl.Core.Communication.Repositories {
    public interface ICommunicationRepository {
        public Task<List<UserConferenceItem>> GetUserActiveConferences();
        Task<string> HandelCallResponse(string callSid, string callStatus, string from, string to, int duration = 0, string operato = "TWILIO");
        Task<CommsStatus> TwilioText(TwilioRequest ip);
        Task<CommsStatus> TwilioCall(TwilioRequest ip);
        Task<MessageResource> TwilioTextLog(TwilioRequest ip);
        Task<CallResource> TwilioCallLog(TwilioRequest ip);
        Task<ConferenceResource> TwilioConfLog(TwilioRequest ip);
        Task<ResourceSet<RecordingResource>> TwilioRecordingLog(TwilioRequest ip);
        Task<VerificationResource> TwilioVerify(TwilioRequest ip);
        Task<VerificationCheckResource> TwilioVerifyCheck(TwilioRequest ip);
        Task<dynamic> TwilioEndConferenceCall(TwilioRequest ip);
        Task DeleteRecording(string recordingId);
        Task<string> RejoinConference(int confCallId, int companyID, int userID, string source = "APP");
        Task<dynamic> InitComms(string api_CLASS, string aPIClass = "", string clientId = "", string clientSecret = "", string dataCenter = "dublin");
        Task<string> MakeConferenceCall(string fromNumber, string callBackUrl, string messageXML, dynamic commsAPI, string callId,
            string mobileNumber, string landLineNumber, string status, string calledOn);
        Task<string> StartConferenceNew(int companyId, int userId, List<User> userList, string timeZoneId, int ActiveIncidentId = 0, int MessageId = 0, string ObjectType = "Incident");
        Task LogUndelivered(int messageId, string method, int messageDeviceId, int attempt, int companyId, string timeZoneId);
        Task<string> HandelUnifonicCallResponse(string referenceId, string callSId, string callStatus, string From, string To, double CallTimestamp, int Duration = 0, string Operator = "TWILIO");
        Task<string> HandelSMSResponse(string messageSid, string smsStatus, string from, string to, string body, string operato = "TWILIO");
        Task<string> HandelTwoFactor(string messageSid, SmsStatus smsStatus);
        Task<string> TwilioCallAck(HttpContext context);
        Task<string> HandelPushResponse(int sendBackId);
        Task<string> HandelConfResponse(string callSid, string conferenceSid, string statusCallbackEvent);
        Task<dynamic> EndConferenceCall(string conferenceId);
        Task<string> CallConfParticipants(int confCallId, string source = "APP");
        Task<string> HandelConfRecording(string conferenceSid, string recordingSid, string recordingUrl, string recordingStatus,
            int recordingFileSize, int duration);
        Task<HttpResponseMessage> DownloadRecording(string fileName);
    }
}
