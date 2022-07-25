using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class MessageISDList
    {
        public int MessageID { get; set; }
        public string ISDCode { get; set; }
        public int SMSCount { get; set; }
        public int PhoneCount { get; set; }
    }
}
