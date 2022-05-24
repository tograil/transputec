using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;

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

    Task CopyMessageResponse(int CompanyID, int CurrentUserId, string TimeZoneID, CancellationToken token);
    Task<List<UserMessageList>> GetMessages(int targetUserId, string? messageType, int incidentActivationId);
    Task<AcknowledgeReturn> AcknowledgeMessage(int UserID, int MessageID, int MessageListID, string Latitude, string Longitude, string AckMethod, int ResponseID, string TimeZoneId);
    Task<MessageAckDetails> MessageAcknowledged(int CompanyId, int MsgListId, string TimeZoneId, string UserLocationLat, string UserLocationLong, int CurrentUserId, int ResponseID = 0, string AckMethod = "WEB");
    Task<List<IIncidentMessages>> _get_incident_message(int CompanyId, int CurrentUserId);
    Task<List<IPingMessage>> _get_ping_message(int CompanyId, int CurrentUserId);
    Task<List<NotificationDetails>> MessageNotifications(int CompanyId, int CurrentUserId);
    Task<string> LookupWithKey(string Key, string Default = "");
    Task<int> GetCallbackOption(string AckMethod);
    void CreateSOSAlert(int UserID, string SOSType, int MessageId, int MessageListId, int ResponseID, int IncidentActivationId,
            string ResponseLabel, DateTimeOffset UpdatedOn, DateTimeOffset ResponseTimeGMT, string Lat, string Lng, int CallbackOption);
    void CheckSOSAlert(int MessageListID, string SOSType, int CallbackOption);
  

    Task<IncidentMessageDetails> GetMessageDetails(string CloudMsgId, int MessageId = 0);
    Task<List<MessageAttachment>> GetMessageAttachment(int MessageListID, int MessageID);
    Task<List<MessageAttachment>> GetAttachment(int MessageAttachmentID = 0);
}