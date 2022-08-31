using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support
{
    public class CommsMethodModel
    {
        public int MethodId { get; set; }
        public string MessageType { get; set; }
        public int Priority { get; set; }
        public string MethodName { get; set; }
    }
}
