using System;

namespace CrisesControl.Core.Models
{
    public partial class MonthlyTransactionItem
    {
        public int TransactionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public decimal ItemValue { get; set; }
        public string? UserRole { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public bool ThisMonthOnly { get; set; }
    }
}
