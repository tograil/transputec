using System;

namespace CrisesControl.Core.Models
{
    public partial class TwoFactorAuthLog
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string ToNumber { get; set; } = null!;
        public string? CloudMessageId { get; set; }
        public bool LogCollected { get; set; }
        public bool IsBilled { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
