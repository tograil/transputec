using System;

namespace CrisesControl.Core.Models
{
    public partial class TwilioPricing
    {
        public int Id { get; set; }
        public string ChannelType { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public string CountryIso2 { get; set; } = null!;
        public string DesinationPrefix { get; set; } = null!;
        public decimal BasePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public string FriendlyName { get; set; } = null!;
        public string? NumberType { get; set; }
        public DateTimeOffset? UpdateTime { get; set; }
    }
}
