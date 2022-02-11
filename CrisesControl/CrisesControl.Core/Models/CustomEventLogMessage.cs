using System;

namespace CrisesControl.Core.Models
{
    public partial class CustomEventLogMessage
    {
        public long EventLogMessageId { get; set; }
        public long EventLogId { get; set; }
        public int MessageId { get; set; }
        public string? MessageText { get; set; }
        public int? RcpntUserId { get; set; }
        public int? RcpntGroupId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
