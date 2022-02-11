using System;

namespace CrisesControl.Core.Models
{
    public partial class TwilioLogBatch
    {
        public int BatchId { get; set; }
        public string? LogType { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string? CommsProvider { get; set; }
    }
}
