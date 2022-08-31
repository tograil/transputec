using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support
{
    public class IncidentMessagesRtn
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public int Source { get; set; }
        public string MessageType { get; set; }
        public IncidentTaskNotes Notes { get; set; }
        public string RecordingSid { get; set; }
        public string SentByFirst { get; set; }
        public string SentByLast { get; set; }
        public UserFullName SentBy { get; set; }
        public int SentByID { get; set; }
        public string UserPhoto { get; set; }
        public int TotalAcknowledged { get; set; }
        public int TotalNotAcknowledged { get; set; }
        public DateTimeOffset MessageCreatedOn { get; set; }
        public int SafetyCount { get; set; }
        public DateTimeOffset ConferenceStart { get; set; }
        public DateTimeOffset ConferenceEnd { get; set; }
        public int AttachmentCount { get; set; }
        public int HasReply { get; set; }
        public bool MultiResponse { get; set; }
        public int ParentID { get; set; }
    }
}
