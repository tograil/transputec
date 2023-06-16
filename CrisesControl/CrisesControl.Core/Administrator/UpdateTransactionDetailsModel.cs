using CrisesControl.Core.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class UpdateTransactionDetailsModel: CcBase
    {
        public UpdateTransactionDetailsModel()
        {
            PaymentPeriod = "MONTHLY";
        }
        public int TransactionDetailsId { get; set; }
        public string TransactionReference { get; set; }
        public int TransactionTypeId { get; set; }
        public decimal TransactionRate { get; set; }
        public decimal MinimumPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal LineValue { get; set; }
        public decimal Total { get; set; }
        public decimal Vat { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public int TransactionStatus { get; set; }
        public string DRCR { get; set; }
        public int Storage { get; set; }
        public string PaymentPeriod { get; set; }
        public string PaymentMethod { get; set; }
    }
}
