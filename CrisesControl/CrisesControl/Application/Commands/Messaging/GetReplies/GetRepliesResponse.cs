using CrisesControl.Core.Messages;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Messaging.GetReplies
{
    public class GetRepliesResponse
    {
        public List<MessageDetails> Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
    }
}
