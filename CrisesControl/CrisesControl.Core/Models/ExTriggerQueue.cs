using System;

namespace CrisesControl.Core.Models
{
    public partial class ExTriggerQueue
    {
        public int ExTriggerQueueId { get; set; }
        public string TriggerKey { get; set; } = null!;
        public bool IsLocked { get; set; }
        public string SourceFrom { get; set; } = null!;
        public int Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? EmailGuid { get; set; }
        public string? SourceEmailBody { get; set; }
        public bool IsNewMessage { get; set; }
        public bool HasNewRcpntList { get; set; }
        public string? UsersToNotify { get; set; }
    }
}
