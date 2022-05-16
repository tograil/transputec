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
    public class IIncidentMessages
    {
        public int MessageId { get; set; }
        public int MessageListId { get; set; }
        public int IncidentId { get; set; }
        public string IncidentName { get; set; }
        public int Severity { get; set; }
        public string Location { get; set; }
        public string IncidentIcon { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public string MessageText { get; set; }
        public int RecepientUserId { get; set; }
        public bool MultiResponse { get; set; }
        public bool IsTaskRecepient { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public int AttachmentCount { get; set; }
    }

    public class IPingMessage
    {
        public int MessageId { get; set; }
        public int MessageListId { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public string MessageText { get; set; }
        public int RecepientUserId { get; set; }
        public bool MultiResponse { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public int AttachmentCount { get; set; }
    }
}
