using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class InvoiceSchReturn
    {
        public string ItemName { get; set; }
        public int OrderItemID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int InvYear { get; set; }
        public int InvMonth { get; set; }
        public double TransactionRate { get; set; }
        public double InvoiceAmount { get; set; }
        public double InterestPaid { get; set; }
        public double BalanceAmount { get; set; }
    }
}
