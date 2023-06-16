using CrisesControl.Core.Register;
using CrisesControl.Core.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication.Services
{
    public interface ICommsService
    {
        public int GCompanyId { get; set; }
        public int GUserId { get; set; }
        public string GTimezoneId { get; set; }
        Task<string> StartConference(int companyId, int userId, List<int> userList, string timeZoneId, int activeIncidentId = 0, int messageId = 0, string objectType = "Incident");
        Task<string> StartConferenceNew(int companyId, int userId, List<User> userList, string timeZoneId, int activeIncidentId = 0, int messageId = 0, string objectType = "Incident");
        string MakeConferenceCall(string fromNumber, string callBackUrl, string messageXML, dynamic commsAPI, out string callId,
            string mobileNumber, string landLineNumber, out string status, string calledOn);
        Task<string> SendOTP(string iSD, string mobileNo, string message, string source = "RESET", string method = "TEXT", string userEmail = "");
        Task LogTwoFactorAuth(int companyId, int userId, string toNumber, string cloudMessageId, string status, string timeZoneId);
        Task<CommsStatus> SendText(string iSD, string toNumber, string message, string callbackUrl = "");
        Task<CommsStatus> TwilioVerify(string toNumber, string method);
        Task<CommsStatus> TwilioVerifyCheck(string toNumber, string code);
        Task DeleteRecording(string recordingID);
        Task<CommsStatus> VerificationCall(string mobileNo, string message, bool sendInDirect, string twilioRoutingApi);
        Task<string> CallMe(string mobileNo);
        Task<dynamic> InitComms(string API_CLASS, string APIClass = "", string clientId = "", string clientSecret = "", string dataCenter = "dublin");
    }
}
