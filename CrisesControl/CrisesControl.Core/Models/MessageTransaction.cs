using System;

namespace CrisesControl.Core.Models
{
    public partial class MessageTransaction
    {
        public int MessageTransactionId { get; set; }
        public int? MessageId { get; set; }
        public string? MethodName { get; set; }
        public int? Attempts { get; set; }
        public string? MessageText { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public int? MessageListId { get; set; }
        public string? CloudMessageId { get; set; }
        public string? DeviceAddress { get; set; }
        public bool IsBilled { get; set; }
        public bool LogCollected { get; set; }
        public string? CommsProvider { get; set; }
    }
}
