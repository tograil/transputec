using System;

namespace CrisesControl.Core.Models
{
    public partial class ContentTag
    {
        public int ContentTagId { get; set; }
        public int ContentId { get; set; }
        public int TagId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
