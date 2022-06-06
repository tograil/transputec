using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class GetStarted
    {
        public int Gsid { get; set; }
        public string? SessionId { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? Cisdcode { get; set; }
        public string? SwtchPhone { get; set; }
        public string? Website { get; set; }
        public int TimeZone { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public string? Country { get; set; }
        public string? DepartmentName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Uisdcode { get; set; }
        public string? Mobile { get; set; }
        public string? Llisdcode { get; set; }
        public string? Landline { get; set; }
        public string? Password { get; set; }
        public int Locdone { get; set; }
        public int Depdone { get; set; }
        public int Userdone { get; set; }
        public int Assdone { get; set; }
        public int Incidone { get; set; }
    }
}
