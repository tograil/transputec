using System;

namespace CrisesControl.Core.Models
{
    public partial class IncidentDeActivationReason
    {
        public int IncidentDeActivationReasonId { get; set; }
        public int IncidentActivationId { get; set; }
        public int CompanyId { get; set; }
        public string? Reason { get; set; }
        public string Type { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
