using System;

namespace CrisesControl.Core.Models
{
    public partial class ActiveIncidentAction
    {
        public int ActiveIncidentActionId { get; set; }
        public int CompanyId { get; set; }
        public string ActionDescription { get; set; } = null!;
        public int Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int IncidentActivationId { get; set; }
        public int IncidentActionId { get; set; }
    }
}
