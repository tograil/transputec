using System;

namespace CrisesControl.Core.Models
{
    public partial class PublicAlertMessageList
    {
        public int QueueId { get; set; }
        public int PublicAlertId { get; set; }
        public int MessageId { get; set; }
        public DateTimeOffset? DateSent { get; set; }
        public DateTimeOffset? DateDelivered { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }
        public string? Postcode { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public bool Text { get; set; }
        public bool Phone { get; set; }
        public bool Email { get; set; }
        public int MessageSentStatus { get; set; }
        public int? MessageDelvStatus { get; set; }
        public string? CloudMessageId { get; set; }
    }
}
