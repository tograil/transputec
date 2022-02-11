using System;

namespace CrisesControl.Core.Models
{
    public partial class CompanyKpistat
    {
        public long? Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime? DateValue { get; set; }
        public int? YearName { get; set; }
        public int? MonthName { get; set; }
        public int? DayName { get; set; }
        public int? TotalPingInKpi { get; set; }
        public int? TotalPingOneHour { get; set; }
        public int? TotalPingWithInCutOff { get; set; }
        public int? TotalPingOutKpimax { get; set; }
        public int? TotalPingNoResponse { get; set; }
        public int? TotalPingAck { get; set; }
        public int? TotalPingNotAck { get; set; }
        public int? TotalPingSent { get; set; }
        public int? TotalIncidentInKpi { get; set; }
        public int? TotalIncidentOneHour { get; set; }
        public int? TotalIncidentWithInCutOff { get; set; }
        public int? TotalIncidentOutKpimax { get; set; }
        public int? TotalIncidentNoResponse { get; set; }
        public int? TotalIncidentAck { get; set; }
        public int? TotalIncidentNotAck { get; set; }
        public int? TotalIncidentSent { get; set; }
        public long? TotalCreated { get; set; }
        public int? App { get; set; }
        public int? Email { get; set; }
        public int? Text { get; set; }
        public int? Phone { get; set; }
        public int? Web { get; set; }
        public string? PingKpi { get; set; }
        public string? PingMaxKpi { get; set; }
        public string? IncidentKpi { get; set; }
        public string? IncidentMaxKpi { get; set; }
    }
}
