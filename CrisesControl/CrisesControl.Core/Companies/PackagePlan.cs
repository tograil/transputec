using System;

namespace CrisesControl.Core.Companies
{
    public partial class PackagePlan
    {
        public int PackagePlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public bool IsDefault { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public decimal PackagePrice { get; set; }
        public bool PingOnly { get; set; }
    }
}
