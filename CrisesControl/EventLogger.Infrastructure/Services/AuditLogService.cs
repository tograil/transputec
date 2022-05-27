using EventLogger.Core.AuditLog;
using EventLogger.Core.AuditLog.Services;
using EventLogger.Core.Mongo.Repositories;
using MongoDB.Driver;

namespace EventLogger.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IMongoRepositoryFactory _repositoryFactory;

        public AuditLogService(IMongoRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task SaveAuditLog(AuditLogEntry auditLogEntry)
        {
            var collection = _repositoryFactory.CreateCollection<AuditLogEntry>("auditLog");

            await collection.InsertOneAsync(auditLogEntry);
        }
    }
}