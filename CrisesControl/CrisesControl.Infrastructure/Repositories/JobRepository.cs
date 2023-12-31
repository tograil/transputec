﻿
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Jobs.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Serilog;

namespace CrisesControl.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<JobRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int UserID;
        private int CompanyID;

        public JobRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<JobRepository> logger)
        {
            _context = context;
            _logger = logger;
            this._httpContextAccessor=httpContextAccessor;
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
                return null;
            }
        }
        /* Created a new store procedure
         * ALTER PROCEDURE [dbo].[Pro_Get_Job]
         * @CompanyID INT, @JobID INT
         *   AS
         *   BEGIN
         *       SET NOCOUNT ON;
         *       SELECT   J.JobID AS 'JobID',
         *                J.JobType AS 'Jobs_JobType',
         *                J.JobName AS 'Jobs_JobName',
         *                J.JobDescription AS 'Jobs_JobDescription',
         *                J.CommandLine AS 'Jobs_CommandLine',
         *                J.CommandLineParams AS 'Jobs_CommandLineParams',
         *                J.ActionType AS 'Jobs_ActionType',
         *                J.NextRunDate AS 'Jobs_NextRunDate',
         *                J.NextRunTime AS 'Jobs_NextRunTime',
         *                J.IsEnabled AS 'Jobs_IsEnabled',
         *                J.Locked AS 'Jobs_Locked',
         *                J.LockedBy AS 'Jobs_LockedBy',
         *                J.CreatedDate AS 'Jobs_CreatedDate',
         *                J.UpdatedDate AS 'Jobs_UpdatedDate',
         *                J.UpdatedBy AS 'Jobs_UpdatedBy',
         *               JS.ScheduleID AS 'JobSchedule_ScheduleID',
         *                JS.FrequencyType AS 'JobSchedule_FrequencyType',
         *                JS.FrequencyInterval AS 'JobSchedule_FrequencyInterval',
         *                JS.FrequencySubDayType AS 'JobSchedule_FrequencySubDayType',
         *                JS.FrequencySubDayInterval AS 'JobSchedule_FrequencySubDayInterval',
         *                JS.RecurrenceFactor AS 'JobSchedule_RecurrenceFactor',
         *                JS.ActiveStartDate AS 'JobSchedule_ActiveStartDate',
         *                JS.ActiveStartTime AS 'JobSchedule_ActiveStartTime',
         *                JS.ActiveEndDate AS 'JobSchedule_ActiveEndDate',
         *                JS.ActiveEndTime AS 'JobSchedule_ActiveEndTime',
         *                JS.NextRunDate AS 'JobSchedule_NextRunDate',
         *                JS.NextRunTime AS 'JobSchedule_NextRunTime',
         *                JS.IsActive AS 'JobSchedule_IsActive'
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
                return null;
            }
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
}
