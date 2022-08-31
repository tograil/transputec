using MediatR;

namespace CrisesControl.Api.Application.Commands.Support.ActiveIncidentTasks
{
    public class ActiveIncidentTasksRequest : IRequest<ActiveIncidentTasksResponse>
    {
        public int ActiveIncidentId { get; set; }
        public int ActiveIncidentTaskId { get; set; }
        public int CompanyId { get; set; }
        public bool Single { get; set; }
    }
}
