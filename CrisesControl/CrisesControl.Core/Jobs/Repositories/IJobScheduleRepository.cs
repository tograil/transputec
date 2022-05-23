using System.Threading.Tasks;

namespace CrisesControl.Core.Jobs.Repositories;

public interface IJobScheduleRepository
{
    Task<int> AddJobSchedule(JobSchedule jobSchedule);
}