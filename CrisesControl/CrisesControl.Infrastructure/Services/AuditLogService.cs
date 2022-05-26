using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CrisesControl.Core.AuditLog;
using CrisesControl.Core.AuditLog.Services;
using CrisesControl.SharedKernel.Enums;
using Grpc.Net.Client;
using GrpcAuditLogClient;
using MongoDB.Bson.IO;
using AuditLogEntry = CrisesControl.Core.AuditLog.AuditLogEntry;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace CrisesControl.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly List<SaveChangesAudit> _changes = new();
        private readonly AuditLogGrpc.AuditLogGrpcClient _auditLogGrpcClient;

        public AuditLogService(AuditLogGrpc.AuditLogGrpcClient auditLogGrpcClient)
        {
            _auditLogGrpcClient = auditLogGrpcClient;
        }

        public void AppendDataAudit(SaveChangesAudit saveChangesAudit)
        {
            _changes.Add(saveChangesAudit);
        }

        public IReadOnlyCollection<SaveChangesAudit> GetSaveChangesAudit()
        {
            return _changes;
        }

        public async Task SaveAuditData(AuditLogEntry auditLogEntry)
        {
            await _auditLogGrpcClient.AddLogEntryAsync(new AuditLogValue
            {
                CompanyId = auditLogEntry.CompanyId,
                UserId = auditLogEntry.UserId,
                Request = JsonConvert.SerializeObject(auditLogEntry.Request),
                Response = JsonConvert.SerializeObject(auditLogEntry.Response),
                SaveChangesAudit = JsonConvert.SerializeObject(auditLogEntry.SaveChangesAuditCollection),
                RequestName = auditLogEntry.RequestName
            });
        }
    }
}