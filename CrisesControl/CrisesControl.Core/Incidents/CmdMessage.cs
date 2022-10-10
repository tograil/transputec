using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class CmdMessage
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public int Source { get; set; }

        public string MessageType { get; set; }
        public string SentByFirst { get; set; }
        public string SentByLast { get; set; }
        public int SentByID { get; set; }
        public string UserPhoto { get; set; }
        public int HasReply { get; set; }
        public int MultiResponse { get; set; }
        public int TotalAcknowledged { get; set; }
        public int TotalNotAcknowledged { get; set; }
        public DateTimeOffset MessageCreatedOn { get; set; }
        public int SafetyCount { get; set; }
        public int IncidentStatus { get; set; }
        public int AttachmentCount { get; set; }
        public int MessageListId { get; set; }
        public int MessageAckStatus { get; set; }
      
    }
}
