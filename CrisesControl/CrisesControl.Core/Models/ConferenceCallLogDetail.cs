using System;

namespace CrisesControl.Core.Models
{
    public partial class ConferenceCallLogDetail
    {
        public int ConferenceCallDetailId { get; set; }
        public int ConferenceCallId { get; set; }
        public int UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SuccessCallId { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ConfJoined { get; set; }
        public DateTimeOffset ConfLeft { get; set; }
        public string? Landline { get; set; }
        public string? CalledOn { get; set; }
        public bool IsBilled { get; set; }
    }
}
