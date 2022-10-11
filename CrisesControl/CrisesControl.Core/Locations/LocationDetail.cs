using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Locations {
    public record LocationDetail {
        public int LocationId { get; set; }
        public string? Location_Name { get; set; }
        public string? Lat { get; set; }
        public string? Long { get; set; }
        public string? Desc { get; set; }
        public string? PostCode { get; set; }
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
