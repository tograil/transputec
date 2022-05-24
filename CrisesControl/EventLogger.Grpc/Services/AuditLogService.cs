using Grpc.Core;

namespace EventLogger.Grpc.Services
{
    public class AuditLogService : AuditLog.AuditLogBase
    {
        public AuditLogService()
        {
            
        }

        public override Task<AuditLogResult> AddLogEntry(AuditLogEntry request, ServerCallContext context)
        {
            return base.AddLogEntry(request, context);
        }

        public override Task<AuditLogListResponse> GetLogsByCompany(AuditLogListRequest request, ServerCallContext context)
        {
            return base.GetLogsByCompany(request, context);
        }
    }
}