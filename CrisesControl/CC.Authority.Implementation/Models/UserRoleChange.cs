using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class UserRoleChange
    {
        public int RoleChangeId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string? UserRole { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
