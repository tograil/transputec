namespace EventLogger.Core.AuditLog.Services
{
    public interface IAuditLogService
    {
        Task SaveAuditLog(AuditLogEntry auditLogEntry);
    }
}