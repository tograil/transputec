using System;

namespace CrisesControl.Core.Models
{
    public partial class ObjectMapping
    {
        public int ObjectMappingId { get; set; }
        public int? CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int SourceObjectId { get; set; }
        public int TargetObjectId { get; set; }
        public Object Object { get; set; }
    }
}
