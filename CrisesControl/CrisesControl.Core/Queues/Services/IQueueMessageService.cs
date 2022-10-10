
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CrisesControl.Core.Queues.Services;

public interface IQueueMessageService
{
    T GetMessage<T>(MessageQueueItem baseMessage) where T : MessageQueueItem;
    Task<bool> CreateCommsLogDumpSession(string entry);
    Task MessageDeviceQueue(int messageID, string messageType, int priority, int cascadePlanID = 0);
    Task MessageDevicePublish(int messageID, int priority, string method = "");
    Task<bool> PublishMessageQueue(int messageID, List<string> rabbitHost, string method,
       dynamic devicelist = null, int priority = 1);
    List<string> RabbitHosts(out string VirtualHost);
    Task<List<MessageQueueItem>> GetDeviceQueue(int messageID, string method, int messageDeviceId = 0, int priority = 1);
    Task<dynamic> GetFailedDeviceQueue(int messageId, string method, int messageDeviceId = 0);
    Task<List<string>> RabbitHosts();
    Task<bool> RequeueMessage(int messageId, string method, dynamic devicelist);
    Task<List<MessageQueueItem>> GetPublicAlertDeviceQueue(int messageId, string method);
    Task<bool> PublishPublicAlertQueue(int messageId, List<string> rabbitHost, string method);
    
}