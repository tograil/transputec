using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class CompanyPackageFeatures
    {
        public string ProductCode { get; set; }
        public string ModuleName { get; set; }
        public double Rate { get; set; }
        public string PaymentPeriod { get; set; }
        public double TransactionRate { get; set; }
        public string ModuleType { get; set; }
        public int Units { get; set; }
        public double Amount { get; set; }
    }
}
