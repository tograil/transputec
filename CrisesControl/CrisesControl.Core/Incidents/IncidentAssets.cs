using CrisesControl.Core.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Incidents;

public class IncidentAssets
{
    public int AssetId { get; set; }
    public string AssetTitle { get; set; }
    public string? AssetDescription { get; set; }
    [NotMapped]
    public string? AssetFileName { get; set; }
    [NotMapped]
    public string? AssetPath { get; set; }
    [NotMapped]
    public string? FilePath { get; set; }
    public string? AssetType { get; set; }
    [NotMapped]
    public double AssetSize { get; set; }
    [NotMapped]
    public int AssetTypeId { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public DateTimeOffset? ReviewDate { get; set; }
    [NotMapped]
    public UserFullName AssetOwnerName { get; set; }
}