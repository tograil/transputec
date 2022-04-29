using MediatR;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs
{
    public class GetAllJobsRequest: IRequest<GetAllJobsResponse>
    {
        public int CompanyID { get; set; }
        public int UserID { get; set; }
       
    }
}
