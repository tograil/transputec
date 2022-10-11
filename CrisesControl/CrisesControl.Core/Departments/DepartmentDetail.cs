using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Departments {
    public record DepartmentDetail {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int Status { get; set; }
        [NotMapped]
        public int ObjectMappingId { get; set; }
        public int UserCount { get; set; }
        public int ActiveUserCount { get; set; }
        public List<User> UserList { get; set; }
        [NotMapped]
        public UserFullName CreatedByName { get; set; }
        [NotMapped]
        public UserFullName UpdatedByName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompanyId { get; set; }
        [NotMapped]
        public DateTimeOffset CreatedOn { get; set; }
        [NotMapped]
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
