using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Core.Queues.Services;

public interface IQueueService
{
    Task MessageDeviceQueue(int messageId, MessageType messageType, int priority, int cascadePlanId);

    Task<ICollection<MessageQueueItem>> GetMessageQueue(int messageId, MessageMethod method, int messageDeviceId = 0,
        int priority = 1);

    Task PublishMessage(int messageId, int priority, MessageMethod method = MessageMethod.All);

    Task EnqueueMessage(int messageId, ICollection<string> rabbitHosts, MessageMethod method);

    ICollection<string> RabbitHosts();
    Task<bool> AppInvitationQueue(int CompanyID);
    Task<List<MessageQueueItem>> GetAppUserQueue(int CompanyID);
    Task<List<MessageQueueItem>> GetDeviceQueue(int MessageID, string Method, int MessageDeviceId = 0, int Priority = 1);
    Task<List<MessageQueueItem>> GetFailedDeviceQueue(int MessageID, string Method, int MessageDeviceId = 0);
}