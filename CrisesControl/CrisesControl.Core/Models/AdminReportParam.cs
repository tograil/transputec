namespace CrisesControl.Core.Models
{
    public partial class AdminReportParam
    {
        public int ParamId { get; set; }
        public int ReportId { get; set; }
        public string ParamName { get; set; } = null!;
        public string ParamType { get; set; } = null!;
        public string? DefaultValue { get; set; }
    }
}
