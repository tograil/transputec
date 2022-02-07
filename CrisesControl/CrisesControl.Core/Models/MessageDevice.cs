using System;

namespace CrisesControl.Core.Models
{
    public partial class MessageDevice
    {
        public int MessageDeviceId { get; set; }
        public int CompanyId { get; set; }
        public int MessageId { get; set; }
        public int MessageListId { get; set; }
        public int UserDeviceId { get; set; }
        public string? Method { get; set; }
        public string? DeviceAddress { get; set; }
        public string? DeviceType { get; set; }
        public string? MessageText { get; set; }
        public int Priority { get; set; }
        public string? LockStatus { get; set; }
        public int Attempt { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? DateSent { get; set; }
        public DateTimeOffset? DateDelivered { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? CloudMessageId { get; set; }
        public int AttributeId { get; set; }
        public bool SirenOn { get; set; }
        public bool OverrideSilent { get; set; }
        public string? SoundFile { get; set; }
        public string? MobileIsd { get; set; }
        public string? MobileNo { get; set; }
        public string? UserEmail { get; set; }
    }
}
