using System;

namespace CrisesControl.Core.Models
{
    public partial class IncidentAction
    {
        public int IncidentActionId { get; set; }
        public int IncidentId { get; set; }
        public int CompanyId { get; set; }
        public string? ActionDescription { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? Title { get; set; }
    }
}
