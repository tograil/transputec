using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Assets
{
    public class Assets
    {
        [Key]
        public int AssetId { get; set; }
        public int CompanyId { get; set; }
        public string AssetTitle { get; set; } = null!;
        public string? AssetDescription { get; set; }
        public string? AssetType { get; set; }
        public string? AssetPath { get; set; }
        [NotMapped]
        public string? FilePath { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public double AssetSize { get; set; }
        public int Status { get; set; }
        public int SourceObjectId { get; set; }
        public int AssetTypeId { get; set; }
        public string? SourceFileName { get; set; }
        public int? AssetOwner { get; set; }
        public int? ReminderCount { get; set; }
        public DateTimeOffset? ReviewDate { get; set; }
        public string? ReviewFrequency { get; set; }
    }
}
