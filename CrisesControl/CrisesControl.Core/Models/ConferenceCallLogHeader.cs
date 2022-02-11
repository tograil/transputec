using System;

namespace CrisesControl.Core.Models
{
    public partial class ConferenceCallLogHeader
    {
        public int ConferenceCallId { get; set; }
        public int CompanyId { get; set; }
        public int InitiatedBy { get; set; }
        public int ActiveIncidentId { get; set; }
        public int MessageId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string? CloudConfId { get; set; }
        public bool Record { get; set; }
        public string? RecordingUrl { get; set; }
        public string? ConfRoomName { get; set; }
        public int Duration { get; set; }
        public long RecordingFileSize { get; set; }
        public string? CurrentStatus { get; set; }
        public DateTimeOffset ConfrenceStart { get; set; }
        public DateTimeOffset ConfrenceEnd { get; set; }
        public string? RecordingSid { get; set; }
        public int TargetObjectId { get; set; }
        public string? TargetObjectName { get; set; }
    }
}
