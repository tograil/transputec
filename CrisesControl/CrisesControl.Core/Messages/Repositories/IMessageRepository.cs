using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;

namespace CrisesControl.Core.Messages.Repositories;

public interface IMessageRepository
{
    Task CreateMessageMethod(int messageId, int methodId, int activeIncidentId = 0, int incidentId = 0);

    int GetPushMethodId();

    Task AddUserToNotify(int messageId, ICollection<int> userIds, int activeIncidentId = 0);

    Task SaveActiveMessageResponse(int messageId, ICollection<AckOption> ackOptions, int activeIncidentId = 0);

    Task DeleteMessageMethod(int messageId = 0, int activeIncidentId = 0);

    Task<int> CreateMessage(int companyId, string msgText, string messageType, int incidentActivationId, int priority,
        int currentUserId,
        int source, DateTimeOffset localTime, bool multiResponse, ICollection<AckOption> ackOptions, int status = 0,
        int assetId = 0, int activeIncidentTaskId = 0, bool trackUser = false, bool silentMessage = false,
        int[] messageMethod = null, ICollection<MediaAttachment> mediaAttachments = null, int parentId = 0,
        int messageActionType = 0);
}