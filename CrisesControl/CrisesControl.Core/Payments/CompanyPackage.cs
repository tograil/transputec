using CrisesControl.Core.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Payments
{
    public class CompanyPackage
    {
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public CompanyPackageItems PackageItems { get; set; }
        public TransactionRates TransactionRates { get; set; }
        
    }
}
