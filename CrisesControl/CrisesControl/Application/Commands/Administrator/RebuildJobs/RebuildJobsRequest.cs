using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.RebuildJobs
{
    public class RebuildJobsRequest:IRequest<RebuildJobsResponse>
    {
        public string Company { get; set; }
        public string JobType { get; set; }
    }
}
