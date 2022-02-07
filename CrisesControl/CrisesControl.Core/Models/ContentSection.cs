using System;

namespace CrisesControl.Core.Models
{
    public partial class ContentSection
    {
        public int ContentSectionId { get; set; }
        public string? SectionName { get; set; }
        public int Status { get; set; }
        public string? SectionType { get; set; }
        public int SopheaderId { get; set; }
        public int SectionOrder { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
