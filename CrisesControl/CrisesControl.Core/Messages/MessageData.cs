using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class MessageData
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public int DataCount { get; set; }
    }
}
