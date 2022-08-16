using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Communication
{
    public class TwilioRequest
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string VerifyServiceID { get; set; }
        public bool UseMessagingCopilot { get; set; }
        public string MessagingCopilotId { get; set; }
        public int RetryCount { get; set; }
        public string FromNumber { get; set; }
        public string DvcAddress { get; set; }
        public string MessageBody { get; set; }
        public string Callback { get; set; }
        public bool IsConf { get; set; }
        public bool CommsDebug { get; set; }
        public string MessageXML { get; set; }
        public bool Record { get; set; }
        public string RecordingSid { get; set; }
        public string Sid { get; set; }
        public string Method { get; set; }
        public string Code { get; set; }
        public string DataCenter { get; set; }
    }
}
