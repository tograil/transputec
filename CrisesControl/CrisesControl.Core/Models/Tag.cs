using System;

namespace CrisesControl.Core.Models
{
    public partial class Tag
    {
        public int TagId { get; set; }
        public int TagCategoryId { get; set; }
        public string? TagName { get; set; }
        public string? SearchTerms { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public LibContentTag LibContentTag { get; set; }
    }
}
