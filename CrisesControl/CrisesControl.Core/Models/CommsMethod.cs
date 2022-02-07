using System;

namespace CrisesControl.Core.Models
{
    public partial class CommsMethod
    {
        public int CommsMethodId { get; set; }
        public string? MethodCode { get; set; }
        public string MethodName { get; set; } = null!;
        public DateTimeOffset UpdatedOn { get; set; }
        public int IsDefault { get; set; }
    }
}
