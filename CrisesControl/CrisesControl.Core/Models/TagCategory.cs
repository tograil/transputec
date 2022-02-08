using System;

namespace CrisesControl.Core.Models
{
    public partial class TagCategory
    {
        public int TagCategoryId { get; set; }
        public string? TagCategoryName { get; set; }
        public string? TagCategorySearchTerms { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
