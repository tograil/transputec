using CrisesControl.Core.Messages;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList
{
    public class GetMessageGroupListResponse
    {
        public List<MessageGroupObject> data { get; set; }
        public string Message { get; set; }
    }
}
