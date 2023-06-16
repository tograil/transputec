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
        public List<IIncidentMessages> IncidentMessages { get; set; }
        public List<IPingMessage> PingMessage { get; set; }
    }
   
    
}
