using System;

namespace CrisesControl.Core.Models
{
    public partial class SecurityGroup
    {
        public int SecurityGroupId { get; set; }
        public int CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? UserRole { get; set; }
    }
}
