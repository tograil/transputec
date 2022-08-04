using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class UsageGraph
    {
        public string MethodName { get; set; }
        public string UsageMonth { get; set; }
        public int RptMonth { get; set; }
        public int RptYear { get; set; }
        public decimal Total { get; set; }
        public int Quantity { get; set; }
    }
}
