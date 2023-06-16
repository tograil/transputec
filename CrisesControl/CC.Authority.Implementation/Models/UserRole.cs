using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string RoleCode { get; set; } = null!;
    }
}
