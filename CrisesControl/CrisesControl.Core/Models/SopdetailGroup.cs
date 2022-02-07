using System;

namespace CrisesControl.Core.Models
{
    public partial class SopdetailGroup
    {
        public int SopdetailGroupId { get; set; }
        public int SopgroupId { get; set; }
        public int SopdetailId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
