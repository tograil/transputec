namespace CrisesControl.Core.Models
{
    public partial class CompanyPackageFeature
    {
        public int PackageFeatureId { get; set; }
        public int CompanyId { get; set; }
        public int SecurityObjectId { get; set; }
        public int Status { get; set; }
        public bool IsPaid { get; set; }
    }
}
