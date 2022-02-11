using System;

namespace CrisesControl.Core.Models
{
    public partial class TransactionHeader
    {
        public int TransactionHeaderId { get; set; }
        public decimal Total { get; set; }
        public string? CompanyPaymentCode { get; set; }
        public string? TransactionReferenceNumber { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public decimal NetTotal { get; set; }
        public decimal VatRate { get; set; }
        public decimal Vatvalue { get; set; }
        public DateTimeOffset TransactionStartDate { get; set; }
        public DateTimeOffset TransactionEndDate { get; set; }
        public int CompanyId { get; set; }
        public DateTimeOffset StatementDate { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal CreditLimit { get; set; }
        public int AdminLimit { get; set; }
        public int AdminUsers { get; set; }
        public int StaffLimit { get; set; }
        public int StaffUsers { get; set; }
        public int StorageLimit { get; set; }
        public double StorageSize { get; set; }
    }
}
