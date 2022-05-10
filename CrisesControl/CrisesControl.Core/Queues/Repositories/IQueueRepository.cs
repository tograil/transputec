using System.Threading.Tasks;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Core.Queues.Repositories;

public interface IQueueRepository
{
    Task CreateMessageQueue(int messageId, MessageType messageType, int priority, int cascadePlanId = 0);
}