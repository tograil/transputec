using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Payments
{
    public class TransactionRates
    {
        public string TransactionCode { get; set; }
        public string TransactionDescription { get; set; }
        public decimal Rate { get; set; }
    }
}
