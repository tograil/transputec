using System;

namespace CrisesControl.Core.Models
{
    public partial class MessageProcessLog
    {
        public int LogId { get; set; }
        public int? MessageId { get; set; }
        public string? EventName { get; set; }
        public string? MethodName { get; set; }
        public string? QueueName { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
