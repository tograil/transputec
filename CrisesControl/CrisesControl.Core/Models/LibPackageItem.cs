namespace CrisesControl.Core.Models
{
    public partial class LibPackageItem
    {
        public int LibPackageItemId { get; set; }
        public int PackagePlanId { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string ItemValue { get; set; } = null!;
        public string? ItemDescription { get; set; }
        public int Status { get; set; }
    }
}
