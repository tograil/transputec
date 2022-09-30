using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages.Services;

public interface IMessageService
{
     bool TextUsed { get; set; }
    
     bool PhoneUsed { get; set; }
     bool EmailUsed { get; set; }
     bool PushUsed { get; set; }
     string TimeZoneId { get; set; }
     int CascadePlanID { get; set; }
     string MessageSourceAction { get; set; }
    Task ProcessMessageMethod(int messageId, int[] messageMethod, int incidentActivationId, bool trackUser = false);
    Task CreateMessageMethod(int MessageID, int MethodID, int ActiveIncidentID = 0, int IncidentID = 0);
    Task AddUserLocation(int userID, int userDeviceID, double latitude, double longitude, string locationAddress,
        DateTimeOffset userDeviceTime);
    Task AddUserToNotify(int messageId, List<int> userId, int activeIncidentId = 0);
    Task<int> CreateMessage(int companyId, string msgText, string messageType, int incidentActivationId, int priority, int currentUserId,
       int source, DateTimeOffset localTime, bool multiResponse, List<AckOption> ackOptions, int status = 0, int assetId = 0, int activeIncidentTaskId = 0,
       bool trackUser = false, bool silentMessage = false, int[] messageMethod = null, List<MediaAttachment> mediaAttachments = null, int parentId = 0,
       int messageActionType = 0);
    Task<string> GetCascadeChannels(List<string> channels);
    Task<bool> CanSendMessage(int companyId);
    Task ProcessMessageAttachments(int messageId, List<MediaAttachment> mediaAttachments, int companyId, int createdUserId, string timeZoneId);
    Task CreateMessageAttachment(int messageId, int assetId, int companyId, int createdUserId, string timeZoneId);
    Task<List<MessageDetails>> GetReplies(int parentId, int companyId, int userId, string source = "WEB");
    Task<List<AudioAssetReturn>> GetMessageAudio(int assetTypeId, int companyId, int userId, string Source = "APP");
    Task CreateIncidentNotificationList(int messageId, int incidentActivationId, int mappingId, int sourceId, int currentUserId, int companyId, string timeZoneId);
    Task<int> CreateMediaAttachment(int messageId, string title, string filePath, string originalFileName, decimal fileSize, int attachmentType,
       int messageListId, int createdBy, string timeZoneId);
    Task DeleteMessageMethod(int messageId = 0, int activeIncidentId = 0);
    Task SaveActiveMessageResponse(int messageId, List<AckOption> ackOptions, int incidentActivationId = 0);
    Task CreateProcessQueue(int messageId, string messageType, string method, string state, int priority);
    Task<List<NotificationDetails>> MessageNotifications(int companyId, int currentUserId);
    Task<UserMessageCountModel> MessageNotificationsCount(int UserID);
    Task<List<IIncidentMessages>> _get_incident_message(int userId, int companyId);
    Task<List<IPingMessage>> _get_ping_message(int userId, int companyId);
    Task<bool> StartConference(List<User> userList, int objectId, int currentUserId, int companyId, string timeZoneId);
    Task<string> StartConferenceNew(int companyId, int userId, List<User> userList, string timeZoneId, int activeIncidentId = 0, int messageId = 0, string objectType = "Incident");
    Task<string> MakeConferenceCall(string fromNumber, string callBackUrl, string messageXML, dynamic commsAPI, string callId,
        string mobileNumber, string landLineNumber, string status, string calledOn);
    Task<int> CreateConferenceHeader(int companyId, int createdBy, string confRoom, bool record, int objectId = 0, int messageId = 0, string objectType = "Incident");
    Task<int> CreateConferenceDetail(string action, int conferenceCallId, int userId, string phoneNumber, string landline, string successCallId,
        string status, string timeZoneId, int confDetailId, string calledOn = "");
    Task<int> CreateMessageList(int messageId, int recipientId, bool isTaskRecepient, bool textUsed, bool phoneUsed, bool emailUsed, bool pushUsed,
           int currentUserId, string timeZoneId);
    Task<int> SaveMessageResponse(int responseId, string responseLabel, string description, bool isSafetyResponse,
                 string safetyAckAction, string messageType, int status, int currentUserId, int companyId, string timeZoneId);
    Task<int> CreateMessageResponse(string responseLabel, bool sosEvent, string messageType, string safetyAction, int status,
        int currentUserId, int companyId, string timeZoneId);
    Task<List<UserLocation>> GetUserMovements(int UserID);
    Task<double> GetDistance(double lat1, double lon1, double lat2, double lon2);
    Task<bool> UserInRange(double locLat, double locLan, double userLat, double userLan, double range);
    double ToRadians(double deg);
    Task AddTrackMeUsers(int incidentActivationId, int messageId, int companyId);
    Task<bool> CalculateMessageCost(int companyId, int messageId, string MessageText);
    Task<List<TwilioPriceList>> GetTwiliPriceList();
    Task<List<MessageISDList>> MessageISDList(int messageId);
    Task HandleMessageMethods(int MessageID);
    Task SavePublicAlertMessageList(string sessionId, int publicAlertId, int messageId, DateTimeOffset dateSent, int textAdded, int emailAdded, int phoneAdded);
    Task<AcknowledgeReturn> AcknowledgeMessage(int userID, int messageID, int messageListID, string latitude, string longitude, string ackMethod, int ResponseID, string timeZoneId);
    Task SocialPosting(int messageId, List<string> socialHandle, int companyId);
    Task TwitterPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret);
    Task LinkedInPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret);
    Task FacekbookPost(string messageText, string consumerKey, string consumerSecret, string authToken, string authSecret);
    Task DownloadRecording(string recordingSid, int companyId, string recordingUrl);


}