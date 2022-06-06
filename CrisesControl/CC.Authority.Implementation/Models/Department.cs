using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class Department
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
    }
}
