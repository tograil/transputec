using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class TransactionList
    {
        public List<TransactionDtls> transactionDtls { get; set; }
        public List<TransactionDetail> TransactionDetail { get; set; }
    }
}
