using System;

namespace CrisesControl.Core.Queues;

public class MessageQueueItem
{
    public int MessageDeviceId { get; set; }
    public string Method { get; set; }
    public int MessageId { get; set; }
    public int MessageListId { get; set; }
    public string DeviceAddress { get; set; }
    public string MessageText { get; set; }
    public string LockStatus { get; set; }
    public int Attempt { get; set; }
    public string Status { get; set; }
    public string MessageType { get; set; }
    public DateTimeOffset CreatedTimeZone { get; set; }
    public bool IsTaskRecepient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int CompanyId { get; set; }
    public int IncidentActivationId { get; set; }
    public string DeviceType { get; set; }
    public bool TrackUser { get; set; }
    public int CreatedBy { get; set; }
    public int UserId { get; set; }
    public string ISDCode { get; set; }
    public string MobileNo { get; set; }
    public bool MultiResponse { get; set; }
    public string Company_Name { get; set; }
    public string CustomerId { get; set; }
    public string CompanyLogoPath { get; set; }
    public string TaskURL { get; set; }
    public string TaskURLLabel { get; set; }
    public DateTimeOffset DateDelivered { get; set; }
    public DateTimeOffset DateSent { get; set; }
    public DateTimeOffset DateSentGMT { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public string UserLocationLat { get; set; }
    public string UserLocationLong { get; set; }
    public int MessageDelvStatus { get; set; }
    public int MessageSentStatus { get; set; }
    public int UpdatedBy { get; set; }
    public bool SirenON { get; set; }
    public bool OverrideSilent { get; set; }
    public string SoundFile { get; set; }
    public bool SilentMessage { get; set; }
    public string SenderFirstName { get; set; }
    public string SenderLastName { get; set; }
    public string SenderName { get; set; }
    public string UserEmail { get; set; }
    public int ParentID { get; set; }
    public string TransportType { get; set; }
    public int BadgeCount { get; set; }

    //settings
    public bool CommsDebug { get; set; }
    public int MaxAttempt { get; set; }
    public string CloudMessageID { get; set; }
    public string SimulationText { get; set; }
    public int MessageActionType { get; set; }
    public string MessageSourceAction { get; set; }
}