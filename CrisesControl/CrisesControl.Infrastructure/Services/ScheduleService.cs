using System;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Jobs.Repositories;
using CrisesControl.Core.Jobs.Services;
using CrisesControl.Infrastructure.MongoSettings;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace CrisesControl.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly IMongoCollection<Job> _jobCollection;
    private readonly IMongoCollection<JobQueueData> _scheduleCollection;

    private readonly IJobRepository _jobRepository;

    public ScheduleService(IOptions<JobsMongoSettings> bookStoreDatabaseSettings,
        IJobRepository jobRepository)
    {
        var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);


        _jobRepository = jobRepository;
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);

        _jobCollection = mongoDatabase.GetCollection<Job>(
            "jobs");
        _scheduleCollection = mongoDatabase.GetCollection<JobQueueData>("jobQueue");
    }

    public event Action<JobQueueData>? OnJobCreated;

    public async Task ScheduleIncident(JobQueueData incident)
    {
        await _scheduleCollection.InsertOneAsync(incident);
    }

    public async Task StartJobsListener()
    {
        try
        {
            var cursor = await _scheduleCollection.WatchAsync();

            await Task.Run(async () =>
            {
                while (await cursor.MoveNextAsync())
                {
                    if (OnJobCreated != null && cursor.Current.Any())
                    {
                        foreach (var changeStreamDocument in cursor.Current)
                        {
                            OnJobCreated.Invoke(changeStreamDocument.FullDocument);
                        }
                    }

                    await Task.Delay(1000);
                }
            });
        }
        catch (Exception e)
        {
            throw;
        }
        
    }
}