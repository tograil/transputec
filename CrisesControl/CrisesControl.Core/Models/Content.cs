using System;

namespace CrisesControl.Core.Models
{
    public partial class Content
    {
        public int ContentId { get; set; }
        public string? ContentType { get; set; }
        public string? ContentText { get; set; }
        public int Status { get; set; }
        public int PrimaryContentId { get; set; }
        public string? Checksum { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
