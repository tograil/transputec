using System;

namespace CrisesControl.Core.Models
{
    public partial class MessageAttachment
    {
        public int MessageAttachmentId { get; set; }
        public int MessageId { get; set; }
        public int? MessageListId { get; set; }
        public string Title { get; set; } = null!;
        public int? AttachmentType { get; set; }
        public string? FilePath { get; set; }
        public decimal? FileSize { get; set; }
        public string? OriginalFileName { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
    }
}
