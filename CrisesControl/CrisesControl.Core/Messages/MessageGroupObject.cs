using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class MessageGroupObject
    {
        public int MappingID { get; set; }
        public int ObjectID { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
    }
}
