using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Scheduler.Repositories
{
    public interface ISchedulerRepository
    {
        Task<IEnumerable<JobSchedulerVM>> GetAllJobs(int CompanyID, int UserID);
        Task<IEnumerable<JobSchedulerVM>> GetJob(int CompanyID, int JobId);
    }
}
