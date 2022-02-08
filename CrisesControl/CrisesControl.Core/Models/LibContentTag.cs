using System;

namespace CrisesControl.Core.Models
{
    public partial class LibContentTag
    {
        public int LibContentTagId { get; set; }
        public int LibContentId { get; set; }
        public int TagId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
