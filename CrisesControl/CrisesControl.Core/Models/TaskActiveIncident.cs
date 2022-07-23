using CrisesControl.Core.Incidents;
using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskActiveIncident
    {
        public int ActiveIncidentTaskId { get; set; }
        public int IncidentTaskId { get; set; }
        public int ActiveIncidentId { get; set; }
        public int CompanyId { get; set; }
        public int TaskOwnerId { get; set; }
        public DateTimeOffset TaskEscalatedDate { get; set; }
        public DateTimeOffset TaskAcceptedDate { get; set; }
        public int TaskStatus { get; set; }
        public int TaskCompletedBy { get; set; }
        public int NextIncidentTaskId { get; set; }
        public int PreviousIncidentTaskId { get; set; }
        public int PreviousOwnerId { get; set; }
        public DateTimeOffset TaskActivationDate { get; set; }
        public int TaskSequence { get; set; }
        public string? TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public bool HasPredecessor { get; set; }
        public double EscalationDuration { get; set; }
        public double ExpectedCompletionTime { get; set; }
        public DateTimeOffset DelayedAccept { get; set; }
        public DateTimeOffset DelayedComplete { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool HasNotes { get; set; }
        public int HasCheckList { get; set; }
        public TaskActiveIncidentParticipant TaskActiveIncidentParticipant { get; set; }
        public IncidentActivation IncidentActivation { get; set; }
    }
}
