using System;

namespace CrisesControl.Core.Models
{
    public partial class IncidentPingGroupRating
    {
        public int CompanyId { get; set; }
        public string Trtype { get; set; } = null!;
        public int NewGroupId { get; set; }
        public DateTime? MessageDate { get; set; }
        public int? TotalPingInKpi { get; set; }
        public int? TotalPingInKpimax { get; set; }
        public int? TotalPingAfterKpimax { get; set; }
        public int? TotalPingAck { get; set; }
        public int? TotalPingSent { get; set; }
        public int? TotalIncidentInKpi { get; set; }
        public int? TotalIncidentInKpimax { get; set; }
        public int? TotalIncidentAfterKpimax { get; set; }
        public int? TotalIncidentAck { get; set; }
        public int? TotalIncidentSent { get; set; }
        public int? TotalIncidentUnAck { get; set; }
        public int? TotalSent { get; set; }
        public long? TotalCreated { get; set; }
        public int GroupType { get; set; }
    }
}
