namespace CrisesControl.Core.Models
{
    public partial class CompanyKpi
    {
        public int CompanyId { get; set; }
        public string? PingKpi { get; set; }
        public string? PingKpimax { get; set; }
        public string? IncidentKpi { get; set; }
        public string? IncidentKpimax { get; set; }
    }
}
