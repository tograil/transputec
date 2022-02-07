using System;

namespace CrisesControl.Core.Models
{
    public partial class FailedLoginAttempt
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string EmailId { get; set; } = null!;
        public string PasswordUsed { get; set; } = null!;
        public string? Ipaddress { get; set; }
        public string? ExtraData { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
