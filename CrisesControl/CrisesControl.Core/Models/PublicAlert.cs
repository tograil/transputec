using System;

namespace CrisesControl.Core.Models
{
    public partial class PublicAlert
    {
        public int PublicAlertId { get; set; }
        public int MessageId { get; set; }
        public bool? Scheduled { get; set; }
        public DateTimeOffset? ScheduleAt { get; set; }
        public int? Executed { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
