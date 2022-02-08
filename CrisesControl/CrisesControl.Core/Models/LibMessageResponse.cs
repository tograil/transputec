namespace CrisesControl.Core.Models
{
    public partial class LibMessageResponse
    {
        public int LibResponseId { get; set; }
        public string? ResponseLabel { get; set; }
        public string? Description { get; set; }
        public string? MessageType { get; set; }
        public bool? IsSafetyOption { get; set; }
        public int Status { get; set; }
    }
}
