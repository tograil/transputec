namespace CrisesControl.Core.Models
{
    public partial class CommsMonitor
    {
        public string? MessageDate { get; set; }
        public string? MessageTime { get; set; }
        public int? EmailFailed { get; set; }
        public int? TextFailed { get; set; }
        public int? PhoneFailed { get; set; }
    }
}
