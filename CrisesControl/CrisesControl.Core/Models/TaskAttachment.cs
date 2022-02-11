namespace CrisesControl.Core.Models
{
    public partial class TaskAttachment
    {
        public int AttachmentId { get; set; }
        public int ActiveTaskId { get; set; }
        public int TaskActionId { get; set; }
        public string FileName { get; set; } = null!;
        public string SourceFileName { get; set; } = null!;
        public double FileSize { get; set; }
    }
}
