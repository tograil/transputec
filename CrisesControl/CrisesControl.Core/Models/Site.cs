using System;

namespace CrisesControl.Core.Models
{
    public partial class Site
    {
        public int SiteId { get; set; }
        public int CompanyId { get; set; }
        public string SiteName { get; set; } = null!;
        public string SiteCode { get; set; } = null!;
        public int Status { get; set; }
        public bool IsPrimary { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
    }
}
