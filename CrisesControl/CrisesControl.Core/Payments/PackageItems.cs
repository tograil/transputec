using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Payments
{
    public class PackageItems
    {
        public int SecurityObjectID { get; set; }

        public string SecurityKey { get; set; }
        public int Status { get; set; }
        public decimal Rate { get; set; }
        public string ChargeType { get; set; }
        public string TransactionTypeName { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionDescription { get; set; }
        public int TransactionTypeID { get; set; }
    }
}
