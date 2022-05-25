using CrisesControl.Core.Messages;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Messaging.MessageAcknowledged
{
    public class MessageAcknowledgeResponse
    {
        public MessageAckDetails MessageAckDetails { get; set; }
        public List<NotificationDetails> MessageListData { get; set; }
        public int MessageListId { get; set; }
        public int ErrorId { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
