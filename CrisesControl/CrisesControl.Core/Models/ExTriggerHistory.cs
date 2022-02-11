using System;

namespace CrisesControl.Core.Models
{
    public partial class ExTriggerHistory
    {
        public int ExTriggerHistoryId { get; set; }
        public int ExTriggerId { get; set; }
        public int IncidentActivationId { get; set; }
        public int PingMessageId { get; set; }
        public string? ActionApplied { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string? SourceEmailBody { get; set; }
    }
}
