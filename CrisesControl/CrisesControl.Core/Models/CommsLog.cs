using System;

namespace CrisesControl.Core.Models
{
    public partial class CommsLog
    {
        public int LogId { get; set; }
        public string? Sid { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public string? Body { get; set; }
        public int? NumSegments { get; set; }
        public string? ToFormatted { get; set; }
        public string? FromFormatted { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public int? Duration { get; set; }
        public decimal Price { get; set; }
        public string? PriceUnit { get; set; }
        public string? Direction { get; set; }
        public string? AnsweredBy { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? CommType { get; set; }
        public string? CommsProvider { get; set; }
    }
}
