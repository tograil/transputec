using CrisesControl.Core.Messages;

namespace CrisesControl.Api.Application.Commands.Messaging.GetReplies
{
    public class GetRepliesResponse
    {
        public IMessageDetails data { get; set; }
        public int ErrorCode { get; set; }
        public string  Message { get; set; }
    }
}
