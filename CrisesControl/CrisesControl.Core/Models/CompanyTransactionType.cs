using System;

namespace CrisesControl.Core.Models
{
    public partial class CompanyTransactionType
    {
        public int CompanyTranscationTypeId { get; set; }
        public int TransactionTypeId { get; set; }
        public decimal TransactionRate { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? PaymentPeriod { get; set; }
        public DateTimeOffset NextRunDate { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
