using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class PasswordChangeHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? LastPassword { get; set; }
        public DateTimeOffset ChangedDateTime { get; set; }
    }
}
