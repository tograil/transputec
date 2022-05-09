using System;
using System.Threading.Tasks;

namespace CrisesControl.Core.Jobs.Services;

public interface IScheduleService
{
    event Action<Job> OnJobCreated;
    Task ScheduleIncident(JobSchedule incident);
    Task<Job> GetNextIncidentJob();

    Task StartJobsListener();
}