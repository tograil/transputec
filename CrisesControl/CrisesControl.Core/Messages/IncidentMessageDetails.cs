using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class IncidentMessageDetails
    {
        public int ActiveIncidentId { get; set; }
        public int ActiveIncidentMessageId { get; set; }
        public int ActiveIncidentMessageListId { get; set; }
        public DateTimeOffset IncidentMessageDateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MessageSentById { get; set; }
        public string UserPhoto { get; set; }
        public string IncidentMessage { get; set; }
        public string TaskLink { get; set; }
        public int AcknowledgedFlag { get; set; }
        public int AttachmentCount { get; set; }
        public int ParentID { get; set; }
        public int ReplyCount { get; set; }
        public int UnAckCount { get; set; }
        public int Status { get; set; }
        //public int MyProperty { get; set; }
        //public int MyProperty { get; set; }
       
    }
}
