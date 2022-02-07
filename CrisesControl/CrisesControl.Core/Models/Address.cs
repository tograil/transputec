using System;

namespace CrisesControl.Core.Models
{
    public partial class Address
    {
        public int AddressId { get; set; }
        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string Postcode { get; set; } = null!;
        public string? CountryCode { get; set; }
        public string AddressType { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int? Status { get; set; }
        public string? AddressLabel { get; set; }
    }
}
