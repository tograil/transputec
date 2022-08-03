using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class AdminTransaction
    {
        public decimal ItemValue { get; set; }
        public bool ThisMonthOnly { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string UserRole { get; set; }
        public string TransactionDescription { get; set; }
        public string TransactionTypeName { get; set; }
    }
}
