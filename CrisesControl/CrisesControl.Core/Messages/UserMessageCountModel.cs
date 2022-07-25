using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class UserMessageCountModel
    {
        public int UserId { get; set; }
        public int TotalIncidentUnAck { get; set; }
        public int TotalPingUnAck { get; set; }
        public int TaskCount { get; set; }
        public int PendingTask { get; set; }
        public int ActiveCompletedTask { get; set; }
        public int ActiveOffDuty { get; set; }
    }
}
