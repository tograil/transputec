using System;

namespace CrisesControl.Core.Models
{
    public partial class PingGroupRating
    {
        public int CompanyId { get; set; }
        public int NewGroupId { get; set; }
        public int GroupType { get; set; }
        public DateTime? MessageDate { get; set; }
        public int? TotalPingInKpi { get; set; }
        public int? TotalPingInKpimax { get; set; }
        public int? TotalPingAfterKpimax { get; set; }
        public int? TotalPingAck { get; set; }
        public int? TotalPingSent { get; set; }
        public int? TotalPingUnAck { get; set; }
        public int? TotalSent { get; set; }
        public long? TotalCreated { get; set; }
    }
}
