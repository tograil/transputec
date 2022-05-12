using MediatR;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetJob
{
    public class GetJobRequest : IRequest<GetJobResponse>
    {
        public int CompanyID { get; set; }
        public int JobId { get; set; }
    }
}
