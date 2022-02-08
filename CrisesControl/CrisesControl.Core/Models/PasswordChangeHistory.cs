using System;

namespace CrisesControl.Core.Models
{
    public partial class PasswordChangeHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? LastPassword { get; set; }
        public DateTimeOffset ChangedDateTime { get; set; }
    }
}
