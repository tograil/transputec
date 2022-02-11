using System;

namespace CrisesControl.Core.Models
{
    public partial class OrderHeader
    {
        public int OrderId { get; set; }
        public int CompanyId { get; set; }
        public string? CustomerId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public decimal ContractValue { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public int ContractDuration { get; set; }
        public string Status { get; set; } = null!;
        public int? KeyholderCount { get; set; }
        public int? StaffCount { get; set; }
        public string? TigerOrderNo { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal? NetTotal { get; set; }
        public decimal? VatTotal { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public string? ContractType { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int? Activated { get; set; }
        public decimal? Discount { get; set; }
    }
}
