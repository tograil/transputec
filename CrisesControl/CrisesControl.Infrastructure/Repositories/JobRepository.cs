using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Jobs.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CrisesControl.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly CrisesControlContext _context;
    private readonly ILogger<JobRepository> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private int UserID;
    private int CompanyID;

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
    public async Task<IEnumerable<JobList>> GetAllJobs()
    {
        try
        {
            UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

            var pUserId = new SqlParameter("@UserID", UserID);
            var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
            return await _context.Set<JobList>().FromSqlRaw("EXEC Pro_Get_Schedule_Jobs @CompanyID,@UserID", pCompanyId, pUserId).ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);

        }
        return new List<JobList>();
    }
    /* Created a new store procedure
     * ALTER PROCEDURE [dbo].[Pro_Get_Job]
     * @CompanyID INT, @JobID INT
     *   AS
     *   BEGIN
     *       SET NOCOUNT ON;
     *       SELECT   J.JobID AS 'JobID',
     *                J.JobType AS 'JobType',
     *                J.JobName AS 'JobName',
     *                J.JobDescription AS 'JobDescription',
     *                J.CommandLine AS 'Jobs_CommandLine',
     *                J.CommandLineParams AS 'CommandLineParams',
     *                J.ActionType AS 'ActionType',
     *                J.NextRunDate AS 'NextRunDate',
     *                J.NextRunTime AS 'NextRunTime',
     *                J.IsEnabled AS 'IsEnabled',
     *                J.Locked AS 'Locked',
     *                J.LockedBy AS 'LockedBy',
     *                J.CreatedDate AS 'CreatedDate',
     *                J.UpdatedDate AS 'UpdatedDate',
     *                J.UpdatedBy AS 'UpdatedBy',
     *               JS.ScheduleID AS 'ScheduleID',
     *                JS.FrequencyType AS 'FrequencyType',
     *                JS.FrequencyInterval AS 'FrequencyInterval',
     *                JS.FrequencySubDayType AS 'FrequencySubDayType',
     *                JS.FrequencySubDayInterval AS 'FrequencySubDayInterval',
     *                JS.RecurrenceFactor AS 'RecurrenceFactor',
     *                JS.ActiveStartDate AS 'ActiveStartDate',
     *                JS.ActiveStartTime AS 'ActiveStartTime',
     *                JS.ActiveEndDate AS 'ActiveEndDate',
     *                JS.ActiveEndTime AS 'ActiveEndTime',
     *                JS.NextRunDate AS 'SchedulerNextRunDate',
     *                JS.NextRunTime AS 'SchedulerNextRunTime',
     *                JS.IsActive AS 'IsActive'
     *       FROM     [dbo].[Jobs] AS J WITH (NOLOCK)
     *                INNER JOIN
     *                [dbo].[JobSchedules] AS JS WITH (NOLOCK)
     *                ON J.[JobID] = JS.JobID
     *       WHERE    J.CompanyId = @CompanyID
     *                AND J.JobID = @JobID
     *                AND J.ActionType IN ('Incident', 'Ping', 'LOCATION', 'GROUP', 'DEPARTMENT', 'USER')
     *                AND J.UpdatedBy IN (SELECT UserId
     *                                    FROM   dbo.Get_Segregated_UserList(@CompanyID, JS.UpdatedBy))
     *       ORDER BY J.JobID DESC;
     *       SET NOCOUNT OFF;
     * 
*/
    public async Task<IEnumerable<JobList>> GetJob(int CompanyID, int JobId)
    {
        try
        {


            var jobId = new SqlParameter("@JobID", JobId);
            var companyId = new SqlParameter("@CompanyID", CompanyID);
            var jobs = await _context.Set<JobList>().FromSqlRaw("EXEC Pro_Get_Job @CompanyID, @JobID", companyId, jobId).ToListAsync();
            return jobs;

        }
        catch (Exception ex)
        {
            Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                  ex.Message, ex.StackTrace, ex.InnerException, ex.Source);

        }
        return new List<JobList>();
    }
}