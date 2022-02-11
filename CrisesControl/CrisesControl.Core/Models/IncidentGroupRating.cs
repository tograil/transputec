using System;

namespace CrisesControl.Core.Models
{
    public partial class IncidentGroupRating
    {
        public int CompanyId { get; set; }
        public int NewGroupId { get; set; }
        public int GroupType { get; set; }
        public DateTime? MessageDate { get; set; }
        public int? TotalIncidentInKpi { get; set; }
        public int? TotalIncidentInKpimax { get; set; }
        public int? TotalIncidentAfterKpimax { get; set; }
        public int? TotalIncidentAck { get; set; }
        public int? TotalIncidentSent { get; set; }
        public int? TotalIncidentUnAck { get; set; }
        public int? TotalSent { get; set; }
        public long? TotalCreated { get; set; }
    }
}
