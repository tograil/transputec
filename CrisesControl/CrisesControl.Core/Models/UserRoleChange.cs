using System;

namespace CrisesControl.Core.Models
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
