using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies
{
    public class ReplyChannel
    {
        public string HasLowBalance { get; set; }
        public int MethodId { get; set; }
        public bool ServiceStatus { get; set; }
        public int Status { get; set; }
        public string MethodName { get; set; }
    }
}
