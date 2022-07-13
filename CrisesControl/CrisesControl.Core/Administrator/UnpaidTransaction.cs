using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class UnpaidTransaction
    {
        public string TransactionTypeName { get; set; }
        public string Company_Name { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public decimal Total { get; set; }
        public decimal LineValue { get; set; }
        public decimal LineVat { get; set; }
        public int TransactionHeaderId { get; set; }
        public string TransactionReference { get; set; }
    }
}
