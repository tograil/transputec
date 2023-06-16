using CrisesControl.Core.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing {
    public class BillingPaymentProfile {
        public string PaidServices { get; set; }
        public CompanyPaymentProfile Profile { get; set; }
    }

    public class CompanySubscribedMethod {
        public int MethodId { get; set; }
        public string MethodCode { get; set; }
        public int Status { get; set; }
        public bool ServiceStatus { get; set; }

    }
}
