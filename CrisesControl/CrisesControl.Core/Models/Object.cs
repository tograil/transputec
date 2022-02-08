using System;

namespace CrisesControl.Core.Models
{
    public partial class Object
    {
        public int ObjectId { get; set; }
        public string ObjectName { get; set; } = null!;
        public string ObjectTableName { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public bool IsDefault { get; set; }
    }
}
