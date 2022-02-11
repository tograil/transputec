namespace CrisesControl.Core.Models
{
    public partial class IncidentStat
    {
        public int CompanyId { get; set; }
        public int IncidentActivationId { get; set; }
        public int? TotalIncidentInKpi { get; set; }
        public int? TotalIncidentAck { get; set; }
        public int? TotalIncidentUnAck { get; set; }
        public int? TotalIncidentSent { get; set; }
        public int? TotalIncidentNotInKpi { get; set; }
        public int? TotalIncidentNotInKpimax { get; set; }
        public int? TotalGoldenHour { get; set; }
        public int? TotalWithinCutOff { get; set; }
        public int? TotalAfterCutOff { get; set; }
        public long? TotalCreated { get; set; }
    }
}
