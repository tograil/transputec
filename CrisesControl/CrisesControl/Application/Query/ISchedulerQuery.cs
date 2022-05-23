using CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs;
using CrisesControl.Api.Application.Commands.Scheduler.GetJob;

namespace CrisesControl.Api.Application.Query
{
    public interface ISchedulerQuery
    {
        Task<GetAllJobsResponse> GetAllJobs(GetAllJobsRequest request);
        Task<GetJobResponse> GetJob(GetJobRequest request);
    }
}
