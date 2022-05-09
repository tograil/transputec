using CrisesControl.Core.Jobs;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetJob
{
    public class GetJobResponse
    {
        public List<JobList> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
