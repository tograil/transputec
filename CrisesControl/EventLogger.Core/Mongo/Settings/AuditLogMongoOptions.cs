namespace EventLogger.Core.Mongo.Settings
{
    public class AuditLogMongoOptions
    {
        public const string AuditLogMongo = "AuditLogMongo";

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}