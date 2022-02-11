namespace CrisesControl.Core.Models
{
    public partial class Attachment
    {
        public int AttachmentId { get; set; }
        public string? Title { get; set; }
        public string? OrigFileName { get; set; }
        public string? FileName { get; set; }
        public string? MimeType { get; set; }
        public string? AttachmentType { get; set; }
        public int? ObjectId { get; set; }
        public decimal? FileSize { get; set; }
    }
}
