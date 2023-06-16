using CrisesControl.Core.Messages;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessages {
    public class GetMessagesResponse {
        public List<UserMessageList> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
