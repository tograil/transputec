namespace CrisesControl.Core.Models
{
    public partial class PushMonitor
    {
        public string? MessageDate { get; set; }
        public string? MessageTime { get; set; }
        public int? IPhoneCount { get; set; }
        public int? IPhoneSuccess { get; set; }
        public int? AndroidCount { get; set; }
        public int? AndroidSuccess { get; set; }
        public int? WindowsCount { get; set; }
        public int? WindowsSuccess { get; set; }
        public int? Bbcount { get; set; }
        public int? Bbsuccess { get; set; }
    }
}
