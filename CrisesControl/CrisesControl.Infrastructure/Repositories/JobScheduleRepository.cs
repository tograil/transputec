using System.Threading.Tasks;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Jobs.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.Extensions.Logging;

namespace CrisesControl.Infrastructure.Repositories;

public class JobScheduleRepository : IJobScheduleRepository
{
    private readonly CrisesControlContext _context;
    private readonly ILogger<JobScheduleRepository> _logger;
    public JobScheduleRepository(CrisesControlContext context,
        ILogger<JobScheduleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> AddJobSchedule(JobSchedule jobSchedule)
    {
        await _context.AddAsync(jobSchedule);

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Scheduled new job {jobSchedule.JobId}");

        return jobSchedule.JobId;
    }
}