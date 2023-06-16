using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class Country
    {
        public string CountryCode { get; set; } = null!;
        public int CountryId { get; set; }
        public string? CountryPhoneCode { get; set; }
        public string Name { get; set; } = null!;
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? Iso2code { get; set; }
        public bool? PhoneAvailable { get; set; }
        public bool? Smsavailable { get; set; }
        public string? VoicePriceUrl { get; set; }
        public string? SmspriceUrl { get; set; }
    }
}
