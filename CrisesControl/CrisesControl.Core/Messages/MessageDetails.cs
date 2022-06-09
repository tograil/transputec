using CrisesControl.Core.Incidents;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class MessageDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        [NotMapped]
        public UserFullName SentBy { get; set; }
       // public bool MultiResponse { get; set; }
        public int Priority { get; set; }
        //public int TotalAck { get; set; }
        //public int? TotalNotAck { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public int AttachmentCount { get; set; }
        public bool SentByUser { get; set; }
        public string UserPhoto { get; set; }
        public int HasReply { get; set; }
        public int ReplyCount { get; set; }
        public int ParentID { get; set; }
        public int PendingAck { get; set; }
        public int ActiveIncidentID { get; set; }
        public int AcknowledgedFlag { get; set; }
        public string MessageType { get; set; }
        public int IncidentStatus { get; set; }
    }
}
