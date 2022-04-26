using CrisesControl.Core.Users;
using System;

namespace CrisesControl.Core.Incidents;

public class IncidentAssets
{
    public int AssetId { get; set; }
    public string AssetTitle { get; set; }
    public string AssetDescription { get; set; }
    public string AssetFileName { get; set; }
    public string AssetPath { get; set; }
    public string FilePath { get; set; }
    public string AssetType { get; set; }
    public double AssetSize { get; set; }
    public int AssetTypeId { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public DateTimeOffset? ReviewDate { get; set; }
    public UserFullName AssetOwnerName { get; set; }
}