using System;

namespace CrisesControl.Core.Models
{
    public partial class CustomEventLogDetail
    {
        public long EventLogId { get; set; }
        public int EventLogHeaderId { get; set; }
        public int SrNo { get; set; }
        public DateTimeOffset LogEntryDateTime { get; set; }
        public string? IncidentDetails { get; set; }
        public string? SourceOfInformation { get; set; }
        public int? IsConfirmed { get; set; }
        public string? Cmtaction { get; set; }
        public int? ActionPriority { get; set; }
        public int? ActionUser { get; set; }
        public int? ActionGroup { get; set; }
        public DateTimeOffset? ActionDueBy { get; set; }
        public int? StatusOfAction { get; set; }
        public string? ActionDetail { get; set; }
        public string? Comments { get; set; }
        public DateTimeOffset? ActionedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
