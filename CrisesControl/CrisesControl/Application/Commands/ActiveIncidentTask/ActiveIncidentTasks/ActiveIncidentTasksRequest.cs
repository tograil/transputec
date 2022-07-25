using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks
{
    public class ActiveIncidentTasksRequest:IRequest<ActiveIncidentTasksResponse>
    {
        public int ActiveIncidentID { get; set; }
    }
}
