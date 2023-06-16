using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class UserSecurityGroup
    {
        public int UserId { get; set; }
        public int SecurityGroupId { get; set; }
        public int UserSecurityGroupId { get; set; }
    }
}
