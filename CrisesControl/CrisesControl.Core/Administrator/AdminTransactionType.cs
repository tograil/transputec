using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class AdminTransactionType//: CompanyTranscationType
    {
        public int CompanyTranscationTypeId { get; set; }
        public int TransactionTypeID { get; set; }
        public decimal TransactionRate { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string PaymentPeriod { get; set; }
        public DateTimeOffset NextRunDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionCode { get; set; }
    }
}
