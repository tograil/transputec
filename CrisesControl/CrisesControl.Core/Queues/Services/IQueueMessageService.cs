namespace CrisesControl.Core.Queues.Services;

public interface IQueueMessageService
{
    T GetMessage<T>(MessageQueueItem baseMessage) where T : MessageQueueItem;
}