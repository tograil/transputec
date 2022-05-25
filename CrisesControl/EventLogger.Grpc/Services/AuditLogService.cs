using EventLogger.Core.AuditLog;
using EventLogger.Core.AuditLog.Services;
using Grpc.Core;
using Newtonsoft.Json;

namespace EventLogger.Grpc.Services
{
    public class AuditLogService : AuditLogGrpc.AuditLogGrpcBase
    {
        private readonly IAuditLogService _auditLogService;
        public AuditLogService(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public override Task<AuditLogResult> AddLogEntry(AuditLogValue request, ServerCallContext context)
        {
            var saveData = JsonConvert.DeserializeObject<ICollection<SaveChangesAudit>>(request.SaveChangesAudit);

            return base.AddLogEntry(request, context);
        }

        public override Task<AuditLogListResponse> GetLogsByCompany(AuditLogListRequest request, ServerCallContext context)
        {
            return base.GetLogsByCompany(request, context);
        }
    }
}