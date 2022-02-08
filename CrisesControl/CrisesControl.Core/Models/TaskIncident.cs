using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskIncident
    {
        public int IncidentTaskId { get; set; }
        public int CompanyId { get; set; }
        public int IncidentId { get; set; }
        public int TaskHeaderId { get; set; }
        public int TaskSequence { get; set; }
        public string? TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public bool HasPredecessor { get; set; }
        public double EscalationDuration { get; set; }
        public double ExpectedCompletionTime { get; set; }
        public int Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}
