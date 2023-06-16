using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class Group
    {
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
    }
}
