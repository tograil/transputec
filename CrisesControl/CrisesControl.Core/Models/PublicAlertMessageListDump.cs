using System;

namespace CrisesControl.Core.Models
{
    public partial class PublicAlertMessageListDump
    {
        public int QueueId { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }
        public string? Postcode { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? SessionId { get; set; }
        public int? PublicAlertId { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
    }
}
