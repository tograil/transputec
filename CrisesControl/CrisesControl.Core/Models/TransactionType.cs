using System;

namespace CrisesControl.Core.Models
{
    public partial class TransactionType
    {
        public int TransactionTypeId { get; set; }
        public string TransactionCode { get; set; } = null!;
        public string? TransactionDescription { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string TransactionTypeName { get; set; } = null!;
        public decimal Rate { get; set; }
        public string? ChargeType { get; set; }
    }
}
