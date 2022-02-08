using System;

namespace CrisesControl.Core.Models
{
    public partial class ExTriggerAttachment
    {
        public int ExTriggerAttachmentId { get; set; }
        public int ExTriggerQueueId { get; set; }
        public string? FileName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
