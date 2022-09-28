using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class AcknowledgeReturn
    {
        [NotMapped]
        public int? ActivePingMessageListId { get; set; }
        public int? ActivePingMessageId { get; set; }
        [NotMapped]
        public int? ActiveIncidentID { get; set; }
        [NotMapped]
        public int? ActiveIncidentMessageID { get; set; }
        [NotMapped]
        public int? ActiveIncidentMessageListID { get; set; }
        [NotMapped]
        public int? AcknowledgedFlag { get; set; }
        [NotMapped]
        public DateTimeOffset DateAcknowledged { get; set; }
    }
}
