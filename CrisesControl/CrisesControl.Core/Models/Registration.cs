using System;

namespace CrisesControl.Core.Models
{
    public partial class Registration
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? MobileIsd { get; set; }
        public string? MobileNo { get; set; }
        public string? VerificationCode { get; set; }
        public DateTimeOffset? VerficationExpire { get; set; }
        public string? CompanyName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public string? CountryCode { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string UniqueReference { get; set; } = null!;
        public int Status { get; set; }
        public int? PackagePlanId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? CustomerId { get; set; }
        public string? Sector { get; set; }
    }
}
