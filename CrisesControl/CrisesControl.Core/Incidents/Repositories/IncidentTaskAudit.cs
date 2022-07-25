using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents.Repositories
{
    public class IncidentTaskAudit
    {
        public string TaskTitle { get; set; }
        public DateTimeOffset TaskActivationDate { get; set; }
        public DateTimeOffset TaskAcceptedDate { get; set; }
        public int TaskSequence { get; set; }
        public int TaskStatus { get; set; }
        public DateTimeOffset DelayedAccept { get; set; }
        public DateTimeOffset DelayedComplete { get; set; }
        public int ActiveIncidentTaskID { get; set; }
        public string ActionDescription { get; set; }
        public DateTimeOffset ActionDate { get; set; }
        public string ActionTypeName { get; set; }
        public int TaskActionTypeID { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double EscalationDuration { get; set; }
        public double ExpectedCompletionTime { get; set; }
        public int TaskOwnerID { get; set; }
        public int PredecessorCount { get; set; }
        public int PendingPredecessor { get; set; }
        public string FileName { get; set; }
        public string AttachmentID { get; set; }
        public string Predecessors { get; set; }
        public int HasAttachment { get; set; }
    }
}
