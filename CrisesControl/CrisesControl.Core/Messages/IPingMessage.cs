using CrisesControl.Core.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
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
