using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CustomEventLog
{
    public class EventLogModel
    {
        public int EventLogHeaderId { get; set; }
        public int ActiveIncidentId { get; set; }
        public int PermittedDepartment { get; set; }
    }
}
