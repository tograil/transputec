using System;

namespace CrisesControl.Core.Models
{
    public partial class CompanyPackageItem
    {
        public int CompanyPackageItemId { get; set; }
        public int CompanyId { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemValue { get; set; } = null!;
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
