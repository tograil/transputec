namespace CrisesControl.Core.Models
{
    public partial class AdminReport
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; } = null!;
        public string? Description { get; set; }
        public string ReportSource { get; set; } = null!;
        public string SourceType { get; set; } = null!;
        public string? DownloadFileName { get; set; }
        public bool HasParams { get; set; }
    }
}
