using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class UserLocation
    {
        public int UniqueId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? Lat { get; set; }
        public string? Long { get; set; }
        public string? Desc { get; set; }
        public string? PostCode { get; set; }
        public int LocationStatus { get; set; }
        public int UserStatus { get; set; }
        public bool ReceiveOnly { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
