using CrisesControl.Core.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Groups
{
    public record GroupDetail
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int Status { get; set; }
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
