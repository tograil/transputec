using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskHeader
    {
        public int TaskHeaderId { get; set; }
        public int IncidentId { get; set; }
        public int? Author { get; set; }
        public DateTimeOffset NextReviewDate { get; set; }
        public bool SendReminder { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int ReminderCount { get; set; }
        public string? ReviewFrequency { get; set; }
        public decimal? Rto { get; set; }
        public decimal? Rpo { get; set; }
    }
}
