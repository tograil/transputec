using CrisesControl.Core.Users;
using System;

namespace CrisesControl.Core.Models
{
    public partial class MessageList
    {
        public int MessageListId { get; set; }
        public int MessageId { get; set; }
        public int RecepientUserId { get; set; }
        public string? TransportType { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public DateTimeOffset DateAcknowledge { get; set; }
        public DateTimeOffset DateDelivered { get; set; }
        public string? UserLocationLong { get; set; }
        public string? UserLocationLat { get; set; }
        public int MessageSentStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageDelvStatus { get; set; }
        public string? AckMethod { get; set; }
        public bool IsTaskRecepient { get; set; }

        

        public int ResponseId { get; set; }
        public bool Text { get; set; }
        public bool Phone { get; set; }
        public bool Push { get; set; }
        public bool Email { get; set; }
        public Message Message { get; set; }
        public ActiveMessageResponse ActiveMessageResponse { get; set; }
        public User User { get; set; }
    }
}
