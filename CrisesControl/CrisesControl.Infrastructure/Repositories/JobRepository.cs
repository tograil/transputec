using System.Threading.Tasks;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Jobs.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrisesControl.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly CrisesControlContext _context;
    private readonly ILogger<JobRepository> _logger;

    public JobRepository(CrisesControlContext context,
        ILogger<JobRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> AddJob(Job job)
    {
        await _context.AddAsync(job);

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Added new job {job.JobId}");

        return job.JobId;
    }

    public Task<int> UpdateJob(Job job)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Job> GetJobById(int id)
    {
        return await _context.Set<Job>().FirstAsync(x => x.JobId == id);
    }
}