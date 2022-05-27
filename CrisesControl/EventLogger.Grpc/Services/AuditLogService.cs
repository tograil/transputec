using EventLogger.Core.AuditLog;
using EventLogger.Core.AuditLog.Services;
using Grpc.Core;
using MongoDB.Bson;
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

        public override async Task<AuditLogResult> AddLogEntry(AuditLogValue request, ServerCallContext context)
        {
            var entry = new AuditLogEntry
                {
                    SaveChangesAuditCollection = BsonDocument.Parse($"{{ \"DatabaseChanges\": {request.SaveChangesAudit} }}"),
                    CommandName = request.RequestName,
                    CompanyId = request.CompanyId,
                    Response = BsonDocument.Parse(request.Request),
                    Request = BsonDocument.Parse(request.Request),
                    UserId = request.UserId
                };

            await _auditLogService.SaveAuditLog(entry);

            return new AuditLogResult();
        }

        public override Task<AuditLogListResponse> GetLogsByCompany(AuditLogListRequest request, ServerCallContext context)
        {
            return base.GetLogsByCompany(request, context);
        }
    }
}