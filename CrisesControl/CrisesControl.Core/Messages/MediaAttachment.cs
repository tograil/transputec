namespace CrisesControl.Core.Messages;

public class MediaAttachment
{
    public string Title { get; set; }
    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public decimal FileSize { get; set; }
    public int AttachmentType { get; set; }
}