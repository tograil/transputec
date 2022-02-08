using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskIncidentAction
    {
        public int IncidentTaskActionId { get; set; }
        public int ActiveIncidentTaskId { get; set; }
        public string? ActionDescription { get; set; }
        public DateTimeOffset ActionDate { get; set; }
        public int TaskActionTypeId { get; set; }
        public int TaskActionBy { get; set; }
    }
}
