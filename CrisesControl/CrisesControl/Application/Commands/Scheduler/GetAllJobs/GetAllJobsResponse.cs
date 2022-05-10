using CrisesControl.Core.Models;
using CrisesControl.Core.Jobs;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs
{
    public class GetAllJobsResponse
    {
     public List<JobList> Data { get; set; }
     public string ErrorCode { get; set; }
      
    }
}
