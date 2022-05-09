using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication
{
    public class ConferenceDetails
    {
        public string IncidentName { get; set; }
        public int CloudConfId { get; set; }
        public int ConferenceCallId { get; set; }
        public int UserId { get; set; }
        public string ConfRoomName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CreatedBy { get; set; }


    }
}
