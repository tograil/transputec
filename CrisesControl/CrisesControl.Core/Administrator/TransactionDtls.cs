using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class TransactionDtls
    {
        public int TransactionDetailsId { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionDescription { get; set; }
        public decimal TransactionRate { get; set; }
        public int Quantity { get; set; }
        public decimal LineValue { get; set; }
        public decimal Total { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public int TransactionStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdateOn { get; set; }
        public int TransactionTypeId { get; set; }
        public int TransactionHeaderId { get; set; }
        public decimal LineVat { get; set; }
        public int CompanyId { get; set; }
        public int MessageId { get; set; }
        public decimal Cost { get; set; }
        public decimal MinimumPrice { get; set; }
        public string DRCR { get; set; }
        public bool IsPaid { get; set; }
        public decimal VATRate { get; set; }

    }
}
