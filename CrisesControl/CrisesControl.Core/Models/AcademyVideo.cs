namespace CrisesControl.Core.Models
{
    public partial class AcademyVideo
    {
        public int VideoId { get; set; }
        public string? VideoKey { get; set; }
        public string? VideoTitle { get; set; }
        public string? VideoDescription { get; set; }
        public string? SourceUrl { get; set; }
        public string? SourceType { get; set; }
        public string? VideoImage { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public bool CloseOnEsc { get; set; }
        public int Status { get; set; }
        public int VideoOrder { get; set; }
    }
}
