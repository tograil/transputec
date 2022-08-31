using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support
{
    public class SupportUserResponse
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobileISDCode { get; set; }
        public string? MobileNo { get; set; }
        public string? LLISDCode { get; set; }
        public string? Landline { get; set; }
        public string Primary_Email { get; set; }
        public string UserPassword { get; set; }
        public string? UserPhoto { get; set; }
        public string? UserRole { get; set; }
        public string? UniqueGuiID { get; set; }
        public bool RegisteredUser { get; set; }
        public int Status { get; set; }
        public DateTimeOffset PasswordChangeDate { get; set; }
        public bool? ExpirePassword { get; set; }
        public string UserLanguage { get; set; }
        public bool FirstLogin { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int? DepartmentId { get; set; }
        public int ActiveOffDuty { get; set; }
        public int? TimezoneId { get; set; }
        public bool TrackMe { get; set; }
        public int TrackMeLocation { get; set; }
        public bool Smstrigger { get; set; }
        public UserFullName CreatedByName { get; set; }
        public UserFullName UpdatedByName { get; set; }
        public CommsMethodModel CommsMethod { get; set; }
        public int SecGroup { get; set; }
        public OBJMap OBJMap { get; set; }

    }
}
