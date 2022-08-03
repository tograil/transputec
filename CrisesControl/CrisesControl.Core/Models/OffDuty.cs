using CrisesControl.Core.Users;
using System;

namespace CrisesControl.Core.Models
{
    public partial class OffDuty
    {
        public int OffDutyId { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public string ActivationSource { get; set; } = null!;
        public bool AllowPush { get; set; }
        public bool AllowText { get; set; }
        public bool AllowPhone { get; set; }
        public bool AllowEmail { get; set; }
        public User User { get; set; }
    }
}
