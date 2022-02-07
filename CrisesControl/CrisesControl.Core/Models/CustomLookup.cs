namespace CrisesControl.Core.Models
{
    public partial class CustomLookup
    {
        public int LookupId { get; set; }
        public string? LookupCategory { get; set; }
        public string LookupLabel { get; set; } = null!;
        public string? LookupValue { get; set; }
    }
}
