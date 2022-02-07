using System;

namespace CrisesControl.Core.Models
{
    public partial class SysParameter
    {
        public int SysParametersId { get; set; }
        public string Category { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Display { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
