using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;

namespace CrisesControl.Core.Users
{
    public class User
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public bool RegisteredUser { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? MobileNo { get; set; }
        public string PrimaryEmail { get; set; } = null!;
        public string? SecondaryEmail { get; set; }
        public string? UniqueGuiId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string Password { get; set; } = null!;
        public int Status { get; set; }
        public string? UserPhoto { get; set; }
        public string? Isdcode { get; set; }
        public string? UserRole { get; set; }
        public string? Llisdcode { get; set; }
        public string? Landline { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public DateTimeOffset PasswordChangeDate { get; set; }
        public bool? ExpirePassword { get; set; }
        public string? UserLanguage { get; set; }
        public bool FirstLogin { get; set; }
        public string? Otpcode { get; set; }
        public DateTimeOffset Otpexpiry { get; set; }
        public DateTimeOffset LastLocationUpdate { get; set; }
        public DateTimeOffset TrackingStartTime { get; set; }
        public DateTimeOffset TrackingEndTime { get; set; }
        public int ActiveOffDuty { get; set; }
        public bool Smstrigger { get; set; }
        public int? DepartmentId { get; set; }
        public string? UserHash { get; set; }
        public int? TimezoneId { get; set; }
        //public bool IsValidNumber { get; set; }
        public UserSecurityGroup UserSecurityGroup { get; set; }
        //public UserComm UserComm { get; set; }

        public Company Company { get; set; }
       
    }
}
