using MongoDB.Bson;

namespace EventLogger.Core.AuditLog
{
    public class AuditLogEntry
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string CommandName { get; set; }
        public BsonDocument SaveChangesAuditCollection { get; set; }
        public BsonDocument Request { get; set; }
        public BsonDocument Response { get; set; }
    }
}