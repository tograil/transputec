namespace CrisesControl.Core.Models
{
    public partial class ActiveIncidentAsset
    {
        public int AssetLinkId { get; set; }
        public int AssetId { get; set; }
        public int AssetTypeId { get; set; }
        public string? AssetTitle { get; set; }
        public string? AssetDescription { get; set; }
        public string? AssetType { get; set; }
        public string? AssetPath { get; set; }
        public string? AssetLinkType { get; set; }
        public int ActiveIncidentId { get; set; }
        public int ActiveTaskId { get; set; }
    }
}
