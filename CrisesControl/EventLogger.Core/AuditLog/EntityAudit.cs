using CrisesControl.SharedKernel.Enums;

namespace EventLogger.Core.AuditLog
{
    public class EntityAudit
    {
        public int Id { get; set; }
        public AuditLogType State { get; set; }
        public string AuditMessage { get; set; }

        public SaveChangesAudit SaveChangesAudit { get; set; }
    }
}