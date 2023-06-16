using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.Core.Queues;

namespace CrisesControl.Core.Messages.Services
{
    public interface IMessagingService
    {
        Task EnqueueMessage(ICollection<MessageQueueItem> messages);
    }
}