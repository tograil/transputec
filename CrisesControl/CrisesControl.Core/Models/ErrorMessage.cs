using System;

namespace CrisesControl.Core.Models
{
    public partial class ErrorMessage
    {
        public int ErrorId { get; set; }
        public string? ErrorCode { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? Options { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
