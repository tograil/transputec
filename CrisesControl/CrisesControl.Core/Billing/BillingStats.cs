using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class BillingStats
    {
        public string PropertyName { get; set; }
        public int TotalCount { get; set; }
        public double AssetSize { get; set; }
        public string ObjectType { get; set; }
    }
}
