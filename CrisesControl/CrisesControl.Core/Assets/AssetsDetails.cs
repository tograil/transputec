using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Assets
{
    public class AssetsDetails
    {
        public int? AssetId { get; set; }
        public string? AssetTitle { get; set; }
        public string? AssetDescription { get; set; }
        public string? AssetType { get; set; }
        public string? AssetPath { get; set; }
        public double? AssetSize { get; set; }
        public int? AssetTypeId { get; set; }
        public string? TypeName { get; set; }
        public int? Status { get; set; }
        public string? ReviewFrequency { get; set; }
        public int? AssetOwner { get; set; }
        [NotMapped]
        public UserFullName AssetOwnerName { get; set; }
        [NotMapped]
        public UserFullName CreatedByName { get; set; }
        [NotMapped]
        public UserFullName UpdatedByName { get; set; }
        public DateTimeOffset? ReviewDate { get; set; }
        public string? SourceFileName { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
