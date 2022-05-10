using System.Collections.Specialized;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Jobs.Services;
using CrisesControl.Scheduler.Jobs;
using CrisesControl.SharedKernel.Enums;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi.MongoDbJobStore;

namespace CrisesControl.Scheduler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IScheduleService _scheduleService;

        private IScheduler _scheduler;

        public Worker(ILogger<Worker> logger, IScheduleService scheduleService)
        {
            _logger = logger;
            _scheduleService = scheduleService;

            _scheduleService.OnJobCreated += ScheduleService_OnJobCreated;
        }

        private void ScheduleService_OnJobCreated(Core.Jobs.JobQueueData obj)
        {
            var identity = $"job_{obj.JobData.JobId}_{obj.JobScheduleId}";
            var triggerIdentity = $"trigger_{obj.JobData.JobId}_{obj.JobScheduleId}";
            var group = obj.JobType.ToString();
            var objJobType = obj.JobType switch
            {
                JobType.InitiateIncident => JobBuilder.Create<InitiateIncidentJob>().WithIdentity(identity, group)
                    .UsingJobData("command", JsonConvert.SerializeObject(obj.JobModel as InitiateIncidentModel)).Build(),
                JobType.InitiateAndLaunchIncident => JobBuilder.Create<InitiateAndLaunchJob>().WithIdentity(identity, group)
                    .UsingJobData("command", JsonConvert.SerializeObject(obj.JobModel as InitiateIncidentModel)).Build(),
                JobType.PingMessage => JobBuilder.Create<PingJob>().WithIdentity(identity, group)
                    .UsingJobData("command", JsonConvert.SerializeObject(obj.JobModel as PingMessageModel)).Build()
            };

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerIdentity, group);

            var minutes = obj.FrequencySubInterval / 60;
            var restSeconds = obj.FrequencySubInterval % 60;
            var hours = minutes / 60;
            var restMinutes = minutes % 60;
            var days = hours / 24;
            var restHours = hours % 24;

            trigger = obj.StartDate is null ? trigger.StartNow() : trigger.StartAt(obj.StartDate.Value);

            trigger = obj.FrequencyType switch
            {
                JobFrequencyType.OneTime => trigger.WithSimpleSchedule(x => x.WithRepeatCount(1)),
                JobFrequencyType.Daily => obj.JobSubDayType switch 
                {
                    JobSubDayType.Once =>  trigger.WithCronSchedule($"{restSeconds} {restMinutes} {restHours} ? * *"),
                    JobSubDayType.Hour => trigger.WithCronSchedule($"0 0 0/{hours} ? * *"),
                    JobSubDayType.Minute => trigger.WithCronSchedule($"0 0/{minutes} * ? * *"),
                },
                JobFrequencyType.Weekly => obj.JobSubDayType switch
                {
                    JobSubDayType.Once => trigger.WithCronSchedule($"{restSeconds} {restMinutes} {restHours} {days} * */1"),
                    JobSubDayType.Minute => trigger.WithCronSchedule($"0 0 0/{hours} ? * */1"),
                    JobSubDayType.Hour => trigger.WithCronSchedule($"0 0/{minutes} * ? * */1"),
                },
                JobFrequencyType.Monthly => obj.JobSubDayType switch
                {
                    JobSubDayType.Once => trigger.WithCronSchedule($"{restSeconds} {restMinutes} {restHours} {days} */1 *"),
                    JobSubDayType.Minute => trigger.WithCronSchedule($"0 0 0/{hours} ? */1 *"),
                    JobSubDayType.Hour => trigger.WithCronSchedule($"0 0/{minutes} * ? */1 *"),
                }
            };

            var triggerToStart = trigger.Build();

            _scheduler.ScheduleJob(objJobType, triggerToStart);
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