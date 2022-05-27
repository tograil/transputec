using System.Collections.Generic;

namespace CrisesControl.Core.AuditLog
{
    public class AuditLogEntry
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string RequestName { get; set; }
        public ICollection<SaveChangesAudit> SaveChangesAuditCollection { get; set; }
        public object Request { get; set; }
        public object? Response { get; set; }
    }
}