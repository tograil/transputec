using System;

namespace CrisesControl.Core.Models
{
    public partial class TextMessageLink
    {
        public int MessageLinkId { get; set; }
        public int Identifier { get; set; }
        public int MessageId { get; set; }
        public int MessageListId { get; set; }
        public int UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MessageText { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public bool Acknowledged { get; set; }
    }
}
