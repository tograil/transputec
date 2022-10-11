namespace CrisesControl.Api.Application.Commands.Messaging.PingMessage
{
    public class PingMessageResponse
    {
        public int MessageId { get; set; }
        public string Message { get; set; }
        public bool IsFundAvailable { get; set; }
        public int ErrorId { get; set; }
    }
}
