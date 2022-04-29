using CrisesControl.Core.GroupAggregate;
using CrisesControl.Core.Scheduler;
using CrisesControl.Core.Scheduler.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class SchedulerRepository : ISchedulerRepository
    {
        private readonly CrisesControlContext _context;

        private int UserID;
        public SchedulerRepository(CrisesControlContext context)
        {
         this._context = context;         
        }
        public async Task<IEnumerable<JobSchedulerVM>> GetAllJobs(int CompanyID, int UserID)
        {
            try
            {
                var pUserId = new SqlParameter("@UserID", UserID);
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                return await _context.Set<JobSchedulerVM>().FromSqlRaw("EXEC Pro_Get_Schedule_Jobs @CompanyID,@UserID", pCompanyId, pUserId ).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public async Task<IEnumerable<JobSchedulerVM>> GetJob(int CompanyID, int JobId)
        {
            try
            {

              
                var jobId = new SqlParameter("@JobID", JobId);
                var companyId = new SqlParameter("@CompanyID", CompanyID);
                var jobs= await _context.Set<JobSchedulerVM>().FromSqlRaw("EXEC Pro_Get_Job @CompanyID, @JobID",companyId , jobId).ToListAsync();
                return jobs;                 
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }
    }
}
