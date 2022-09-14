using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CCWebSocket
{
    public class MessageCountResponse
    {
        public int UserId { get; set; }
        public int PingCount { get; set; }
        public int IncidentCount { get; set; }
        public int TaskCount { get; set; }
        public string MessageType { get; set; }
        public string MessageText { get; set; }
    }
}
