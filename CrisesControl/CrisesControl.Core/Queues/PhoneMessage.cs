namespace CrisesControl.Core.Queues;

public class PhoneMessage : MessageQueueItem
{
    public string VoiceAPI { get; set; }
    public string APIClass { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string FromNumber { get; set; }
    public string CallBackUrl { get; set; }
    public string CloudMessageId { get; set; }
    public string MessageXML { get; set; }
    public bool SendInDirect { get; set; }
    public string TwilioRoutingApi { get; set; }
    public string RetryNumberList { get; set; }
}