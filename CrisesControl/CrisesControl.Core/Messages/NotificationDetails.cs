using CrisesControl.Core.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class NotificationDetails
    {
        public IEnumerable<IIncidentMessages> IncidentMessages { get; set; }
        public IEnumerable<IPingMessage> PingMessage { get; set; }
    }
   
    
}
