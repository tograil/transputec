using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Import;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;

namespace CrisesControl.Core.Messages.Repositories;

public interface IMessageRepository
{
    Task CreateMessageMethod(int messageId, int methodId, int activeIncidentId = 0, int incidentId = 0);

    int GetPushMethodId();

    Task AddUserToNotify(int messageId, ICollection<int> userIds, int activeIncidentId = 0);

    Task SaveActiveMessageResponse(int messageId, ICollection<AckOption> ackOptions, int activeIncidentId = 0);

    Task DeleteMessageMethod(int messageId = 0, int activeIncidentId = 0);

    Task<int> CreateMessage(int companyId, string? msgText, string messageType, int incidentActivationId, int priority,
        int currentUserId,
        int source, DateTimeOffset localTime, bool multiResponse, ICollection<AckOption> ackOptions, int status = 0,
        int assetId = 0, int activeIncidentTaskId = 0, bool trackUser = false, bool silentMessage = false,
        int[] messageMethod = null, ICollection<MediaAttachment> mediaAttachments = null, int parentId = 0,
        int messageActionType = 0);
    Task<UserMessageCount> GetNotificationsCount(int currentUserId);
    Task<CompanyMessageResponse> GetMessageResponse(int responseID, string messageType);
    Task<List<CompanyMessageResponse>> GetMessageResponses(string messageType, int Status);
    Task CreateIncidentNotificationList(int incidentActivationId, int messageId,
        ICollection<IncidentNotificationObjLst> launchIncidentNotificationObjLst,
        int currentUserId, int companyId);

    Task CreateIncidentNotificationList(int messageId, int incidentActivationId, int mappingId, int sourceId,
        int currentUserId, int companyId);

    Task<List<LibMessageResponse>> GetLibMessageResponse();

    Task CopyMessageResponse(int companyID, int currentUserId, string timeZoneID, CancellationToken token);
    Task<List<UserMessageList>> GetMessages(int targetUserId, string? messageType, int incidentActivationId);
    Task<AcknowledgeReturn> AcknowledgeMessage(int userID, int messageID, int messageListID, string latitude, string longitude, string ackMethod, int responseID, string timeZoneId);
    Task<MessageAckDetails> MessageAcknowledged(int companyId, int msgListId, string timeZoneId, string userLocationLat, string userLocationLong, int currentUserId, int responseID = 0, string ackMethod = "WEB");
    Task<List<IIncidentMessages>> _get_incident_message(int companyId, int currentUserId);
    Task<List<IPingMessage>> _get_ping_message(int companyId, int currentUserId);
    Task<NotificationDetails> MessageNotifications(int companyId, int currentUserId);
    Task<string> LookupWithKey(string key, string Default = "");
    Task<int> GetCallbackOption(string ackMethod);
    Task CreateSOSAlert(int userID, string sosType, int messageId, int messageListId, int responseID, int incidentActivationId,
            string responseLabel, DateTimeOffset updatedOn, DateTimeOffset responseTimeGMT, string lat, string lng, int callbackOption);
    Task CheckSOSAlert(int messageListID, string sosType, int callbackOption);
  

    Task<IncidentMessageDetails> GetMessageDetails(string cloudMsgId, int messageId = 0);
    Task<List<MessageAttachment>> GetMessageAttachment(int messageListID, int messageID);
    Task<List<MessageAttachment>> GetAttachment(int messageAttachmentID = 0);
    Task<List<MessageDetails>> GetReplies(int parentID, string source = "WEB");
    Task<List<MessageGroupObject>> GetMessageGroupList(int messageID);
    Task<dynamic> ConverToMp3();
    Task<object> GetConfRecordings(int confCallId, int objectId, string objectType, bool single, int companyId);
    Task<dynamic> GetConfUser(int objectId, string objectType);
    Task<PingInfoReturn> GetPingInfo(int messageId, int userId, int companyId);
    dynamic GetPublicAlertTemplate(int templateId, int userId, int companyId);
    Task<List<PublicAlertRtn>> GetPublicAlert(int companyId, int targetUserId);
    Task<int> PingMessages(PingMessageQuery pingMessage);
    dynamic ProcessPAFile(string userListFile, bool hasHeader, int emailColIndex, int phoneColIndex, int postcodeColIndex, int latColIndex, int longColIndex, string sessionId);
    Task<CommonDTO> ResendFailure(int messageId, string commsMethod);
    Task<int> SaveMessageResponse(int responseId, string responseLabel, string description, bool isSafetyResponse, string safetyAckAction, string messageType, int status, int currentUserId, int companyId, string timeZoneId);
    Task<dynamic> SendPublicAlert(string messageText, int[] messageMethod, bool schedulePA, DateTime scheduleAt, string sessionId, int userId, int companyId, string timeZoneId);
    Task<bool> StartConference(List<User> UserList, int ObjectID, int CurrentUserID, int CompanyID, string TimeZoneId);
    public Task<Return> UploadAttachment();
    Task<dynamic> ReplyToMessage(int parentId, string messageText, string replyTo, string messageType, int activeIncidentId, int[] messageMethod,
            int cascadePlanId, int currentUserId, int companyId, string timeZoneId);
}