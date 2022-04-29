using CrisesControl.Core.Scheduler;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetJob
{
    public class GetJobResponse
    {
        public List<JobSchedulerVM> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
