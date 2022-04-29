using CrisesControl.Core.Models;
using CrisesControl.Core.Scheduler;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs
{
    public class GetAllJobsResponse
    {
     public List<JobSchedulerVM> Data { get; set; }
     public string ErrorCode { get; set; }
      
    }
}
