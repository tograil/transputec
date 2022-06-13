using CrisesControl.Core.Messages;

namespace CrisesControl.Api.Application.Commands.Messaging.GetReplies
{
    public class GetRepliesResponse
    {
        public MessageDetails data { get; set; }
        public int ErrorCode { get; set; }
        public string  Message { get; set; }
    }
}
