namespace CrisesControl.Core.Queues;

public class TextMessage : MessageQueueItem
{
    public bool SendOriginalText { get; set; }
    public bool AllowPingAckByText { get; set; }
    public bool AllowIncidentAckbByText { get; set; }
    public bool IncludeSenderInText { get; set; }
    public string SMSAPI { get; set; }
    public string APIClass { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string FromNumber { get; set; }
    public string CallBackUrl { get; set; }
    public string CloudMessageId { get; set; }
    public string CoPilotID { get; set; }
    public bool UseCopilot { get; set; }
    public string GenericText { get; set; }
    public bool AllowPingAckByWA { get; set; }
    public bool AllowIncidentAckbByWA { get; set; }
    public bool IncludeSenderInWA { get; set; }
    public string WAInComingApi { get; set; }
    public string WAStatusCallback { get; set; }
    public bool SendInDirect { get; set; }
    public string TwilioRoutingApi { get; set; }
    public string ReplyToNumber { get; set; }
}