using CrisesControl.Core.Reports;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class IncidentTaskDetails
    {
        public int IncidentTaskID { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public int TaskSequence { get; set; }
        public double EscalationDuration { get; set; }
        public double ExpectedCompletionTime { get; set; }
        public bool HasPredecessor { get; set; }
        public int ActiveIncidentID { get; set; }
        public int ActiveIncidentTaskID { get; set; }
        public DateTimeOffset TaskActivationDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset TaskAcceptedDate { get; set; }
        public int TaskStatus { get; set; }
        public DateTimeOffset DelayedAccept { get; set; }
        public DateTimeOffset DelayedComplete { get; set; }
        public string TaskStatusName { get; set; }
        public int TaskOwnerID { get; set; }
        public bool HasNotes { get; set; }
        public int HasCheckList { get; set; }
        public string? UserPhoto { get; set; }
        [NotMapped]
        public DeclinedList TaskDeclinedBy { get; set; }
        [NotMapped]
        public ReallocatedList TaskReallocatedTo { get; set; }
        [NotMapped]
        public DelegatedList TaskDelegatedTo { get; set; }
        [NotMapped]
        public UserFullName TaskOwner { get; set; }
        [NotMapped]
        public List<int> Recepeints { get; set; }
        [NotMapped]
        public List<int> EscalationRecepeints { get; set; }
        [NotMapped]
        public List<int> ReallocateRecepeints { get; set; }
        [NotMapped]
        public List<int> DelegateRecepeints { get; set; }
        [NotMapped]
        public List<TaskPredecessorList> TaskPredecessor { get; set; }
        [NotMapped]
        public List<TaskPredecessorList> TaskSuccessor { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
