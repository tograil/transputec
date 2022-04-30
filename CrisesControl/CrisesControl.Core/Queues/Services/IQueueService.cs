using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Core.Queues.Services;

public interface IQueueService
{
    Task<ICollection<MessageQueueItem>> GetDeviceQueue(int messageId, MessageMethod method, int messageDeviceId = 0,
        int priority = 1);

    Task PublishMessage(int messageId, int priority, MessageMethod method = MessageMethod.All);

    Task<bool> EnqueueMessage(int messageId, ICollection<string> rabbitHosts, MessageMethod method);

    ICollection<string> RabbitHosts();
}