using CrisesControl.Core.AuditLog;

namespace CrisesControl.Api.Application.Behaviours.Models
{
    public class AuditLogEntry
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public ICollection<SaveChangesAudit> SaveChangesAuditCollection { get; set; }
        public object Request { get; set; }
        public object? Response { get; set; }
    }
}