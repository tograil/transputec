namespace CrisesControl.Core.Models
{
    public partial class PhoneNumberMapping
    {
        public int MappingId { get; set; }
        public string? CountryDialCode { get; set; }
        public string? FromNumber { get; set; }
        public bool AppendToBody { get; set; }
        public string? CommsProvider { get; set; }
        public bool? IsDefault { get; set; }
    }
}
