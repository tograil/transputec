using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class AcknowledgeReturn
    {
        public int ActivePingMessageListId { get; set; }
        public int ActivePingMessageId { get; set; }
        public int ActiveIncidentID { get; set; }
        public int ActiveIncidentMessageID { get; set; }
        public int ActiveIncidentMessageListID { get; set; }
        public int AcknowledgedFlag { get; set; }
        public DateTimeOffset DateAcknowledged { get; set; }
    }
}
