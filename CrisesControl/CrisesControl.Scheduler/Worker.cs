using System.Collections.Specialized;
using CrisesControl.Core.Jobs.Services;
using CrisesControl.Scheduler.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Quartz.Spi.MongoDbJobStore;

namespace CrisesControl.Scheduler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IScheduleService _scheduleService;
        private readonly IJobFactory _jobFactory;

        private IScheduler _scheduler;

        public Worker(ILogger<Worker> logger, IScheduleService scheduleService, IJobFactory jobFactory)
        {
            _logger = logger;
            _scheduleService = scheduleService;
            _jobFactory = jobFactory;

            _scheduleService.OnJobCreated += _scheduleService_OnJobCreated;
        }

        private void _scheduleService_OnJobCreated(Core.Jobs.Job obj)
        {
            var key = new JobKey(obj.JobId.ToString());
            var job = JobBuilder.Create<IncidentJob>()
                .WithIdentity(key)
                .SetJobData(new JobDataMap((IDictionary<string, object>)new Dictionary<string, object>
                {
                    { "JobId", obj.JobId }
                }))
                .Build();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var properties = new NameValueCollection
            {
                [StdSchedulerFactory.PropertySchedulerInstanceName] = "test",
                [StdSchedulerFactory.PropertySchedulerInstanceId] = $"{Environment.MachineName}-{Guid.NewGuid()}",
                [StdSchedulerFactory.PropertyJobStoreType] = typeof(MongoDbJobStore).AssemblyQualifiedName,
                // I treat the database in the connection string as the one you want to connect to
                [$"{StdSchedulerFactory.PropertyJobStorePrefix}.{StdSchedulerFactory.PropertyDataSourceConnectionString}"] = "mongodb://localhost:27017/quartz",
                // The prefix is optional
                [$"{StdSchedulerFactory.PropertyJobStorePrefix}.collectionPrefix"] = "prefix",
                ["quartz.serializer.type"] = "binary"
            };
            var factory = new StdSchedulerFactory(properties);
            _scheduler = await factory.GetScheduler(cancellationToken);

            // and start it off
            await _scheduler.Start(cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _scheduleService.StartJobsListener();

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}