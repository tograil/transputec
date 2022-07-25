using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class TaskIncidentHeader
    {
        public string IncidentName { get; set; }
        public string IncidentIcon { get; set; }
        public int Severity { get; set; }
        public string ImpactedLocation { get; set; }
        public string CurrentStatus { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset AllCompletedOn { get; set; }
        public bool PendingTasks { get; set; }
        [NotMapped]
        public UserFullName LaunchedByName { get; set; }

    }
}
