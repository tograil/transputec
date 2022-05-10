using System;
using System.Threading.Tasks;

namespace CrisesControl.Core.Jobs.Services;

public interface IScheduleService
{
    event Action<JobQueueData> OnJobCreated;
    Task ScheduleIncident(JobQueueData incident);

    Task StartJobsListener();
}