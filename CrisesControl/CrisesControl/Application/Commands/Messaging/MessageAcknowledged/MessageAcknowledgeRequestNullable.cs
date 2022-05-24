namespace CrisesControl.Api.Application.Commands.Messaging.MessageAcknowledged
{
    public class MessageAcknowledgeRequestNullable
    {
        public string? AckMethod { get; set; }
        public string? UserLocationLong { get; set; }
        public string? UserLocationLat { get; set; }
    }
}
