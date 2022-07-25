using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class FailedTaskList
    {
        public string IncidentName { get; set; }
        public int TaskSequence { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int IncidentActivationId { get; set; }
        public int Severity { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public string TaskTitle { get; set; }
        public DateTimeOffset TaskActivationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public UserFullName TaskOwner { get; set; }
        public string CompleteByFirstName { get; set; }
        public string CompleteByLastName { get; set; }
        [NotMapped]
        public UserFullName TaskCompletedBy { get; set; }
        public DateTimeOffset TaskAcceptedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public double AcceptDelay { get; set; }
        public double CompleteDelay { get; set; }
        public int ActiveIncidentTaskID { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string Status { get; set; }
        public int TaskStatus { get; set; }
        public int OverdueMin { get; set; }
        public double EscalationDuration { get; set; }
        public double ExpectedCompletionTime { get; set; }
        public List<TaskPredecessorList> TaskPredecessor { get; set; }
    }
}
