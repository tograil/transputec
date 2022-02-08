namespace CrisesControl.Core.Models
{
    public partial class TwilioLogPushHost
    {
        public int Id { get; set; }
        public string? ApiHost { get; set; }
        public string? LogCollectionUrl { get; set; }
    }
}
