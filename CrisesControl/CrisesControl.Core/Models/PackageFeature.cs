namespace CrisesControl.Core.Models
{
    public partial class PackageFeature
    {
        public int PackageFeatureId { get; set; }
        public int PackagePlanId { get; set; }
        public int SecurityObjectId { get; set; }
    }
}
