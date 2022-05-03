using Quartz;
using CrisesControl.Core.Jobs.Services;

namespace CrisesControl.Scheduler.Jobs;

public class IncidentJob : IJob
{
    private readonly ILogger<IncidentJob> _logger;
    private readonly IScheduleService _scheduleService;

    public IncidentJob(ILogger<IncidentJob> logger,
        IScheduleService scheduleService)
    {
        _logger = logger;
        _scheduleService = scheduleService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Started incident job");

        var jobId = int.Parse(context.JobDetail.Key.Name);

        var job = await _scheduleService.GetNextIncidentJob();

        //TODO: Execute incident

        


    }
}