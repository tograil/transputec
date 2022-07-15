using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrisesControl.Core.Users;

namespace CrisesControl.Core.Incidents
{
    public class UserTaskHead
    {

        public int IncidentActivationId { get; set; }
        public string Name { get; set; }
        public string IncidentIcon { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public string LaunchByFirstName { get; set; }
        public string LaunchByLastName { get; set; }
        [NotMapped]
        public UserFullName LaunchedBy { get; set; }
        public DateTimeOffset DeactivatedOn { get; set; }
        public int DeactivatedBy { get; set; }
        public int TaskCount { get; set; }
        public int CompletedCount { get; set; }
        public int PendingTaskCount { get; set; }
    }
}
