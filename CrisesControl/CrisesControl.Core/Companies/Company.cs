using System;
using System.Collections.Generic;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;

namespace CrisesControl.Core.Companies
{
    public class Company
    {
        public StdTimeZone StdTimeZone { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public int? TimeZone { get; set; }
        public string? CompanyProfile { get; set; }
        public string? SwitchBoardPhone { get; set; }
        public string? Fax { get; set; }
        public string? Website { get; set; }
        public string? PlanDrdoc { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? CompanyLogoPath { get; set; }
        public int Status { get; set; }
        public string? Isdcode { get; set; }
        public int? PackagePlanId { get; set; }
        public DateTimeOffset AnniversaryDate { get; set; }
        public string? IOslogo { get; set; }
        public string? AndroidLogo { get; set; }
        public string? WindowsLogo { get; set; }
        public string? UniqueKey { get; set; }
        public bool OnTrial { get; set; }
        public string? CustomerId { get; set; }
        public string? InvitationCode { get; set; }
        public string? Sector { get; set; }
        public string? ContactLogoPath { get; set; }

        public IEnumerable<User> Users { get; set; }

        public PackagePlan PackagePlan { get; set; }

        public IEnumerable<CompanyPaymentProfile> CompanyPaymentProfiles { get; set; }
        public CompanyActivation CompanyActivation { get; set; }
        public AddressLink AddressLink { get; set; }
        public CompanyPackageItem CompanyPackageItem { get; set; }
    }
}
