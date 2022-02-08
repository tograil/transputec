namespace CrisesControl.Core.Models
{
    public partial class TwilioPriceByIsdcode
    {
        public string? Isdcode { get; set; }
        public string ChannelType { get; set; } = null!;
        public decimal? BasePrice { get; set; }
        public decimal? CurrentPrice { get; set; }
    }
}
