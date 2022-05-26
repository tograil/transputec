using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class MessageAckDetails
    {
        public int MessageListId { get; set; }
        public int ErrorId { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
