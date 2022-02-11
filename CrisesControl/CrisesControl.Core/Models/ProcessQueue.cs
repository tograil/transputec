using System;

namespace CrisesControl.Core.Models
{
    public partial class ProcessQueue
    {
        public int QueueId { get; set; }
        public int MessageId { get; set; }
        public string? Method { get; set; }
        public string State { get; set; } = null!;
        public int Priority { get; set; }
        public string? MessageType { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
