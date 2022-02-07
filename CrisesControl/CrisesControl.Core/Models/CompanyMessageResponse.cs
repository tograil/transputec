using System;

namespace CrisesControl.Core.Models
{
    public partial class CompanyMessageResponse
    {
        public int ResponseId { get; set; }
        public int LibResponseId { get; set; }
        public int CompanyId { get; set; }
        public string? ResponseLabel { get; set; }
        public string? Description { get; set; }
        public string? MessageType { get; set; }
        public bool IsSafetyResponse { get; set; }
        public string? SafetyAckAction { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int Status { get; set; }
    }
}
