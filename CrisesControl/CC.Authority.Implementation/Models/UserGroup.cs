using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class UserGroup
    {
        public int UniqueId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public int UserStatus { get; set; }
        public int GroupStatus { get; set; }
        public bool ReceiveOnly { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
