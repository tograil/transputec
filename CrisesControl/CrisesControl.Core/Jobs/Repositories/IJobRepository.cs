using System.Threading.Tasks;

namespace CrisesControl.Core.Jobs.Repositories;

public interface IJobRepository
{
    Task<int> AddJob(Job job);
    Task<int> UpdateJob(Job job);
    Task<Job> GetJobById(int id);
}