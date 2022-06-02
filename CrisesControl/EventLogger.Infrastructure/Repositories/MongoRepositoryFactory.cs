using EventLogger.Core.Mongo.Repositories;
using EventLogger.Core.Mongo.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace EventLogger.Infrastructure.Repositories
{
    public class MongoRepositoryFactory : IMongoRepositoryFactory
    {
        private AuditLogMongoOptions _options;

        public MongoRepositoryFactory(IOptions<AuditLogMongoOptions> options)
        {
            _options = options.Value;
        }

        public IMongoCollection<T> CreateCollection<T>(string collectionName)
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);


            var mongoClient = new MongoClient(
                _options.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                _options.DatabaseName);

            return mongoDatabase.GetCollection<T>(collectionName);
        }
    }
}