using MongoDB.Driver;

namespace EventLogger.Core.Mongo.Repositories
{
    public interface IMongoRepositoryFactory
    {
        IMongoCollection<T> CreateCollection<T>(string collectionName);
    }
}