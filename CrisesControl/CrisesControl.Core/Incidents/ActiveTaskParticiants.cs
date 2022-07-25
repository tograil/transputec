using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class ActiveTaskParticiants
    {
        public int ParticipantUserID { get; set; }
        public int UserId { get; set; }
        public int ParticipantTypeID { get; set; }
        public int ActiveIncidentTaskID { get; set; }
        public string ActionStatus { get; set; }
        public int ActiveIncidentTaskParticipantID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
