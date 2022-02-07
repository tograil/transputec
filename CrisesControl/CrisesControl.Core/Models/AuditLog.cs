using System;

namespace CrisesControl.Core.Models
{
    public partial class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset EventDateUtc { get; set; }
        public string EventType { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public int RecordId { get; set; }
        public string ColumnName { get; set; } = null!;
        public string? OriginalValue { get; set; }
        public string? NewValue { get; set; }
        public int CompanyId { get; set; }
    }
}
