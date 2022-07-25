using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask
{
    public class DeclineTaskRequest:IRequest<DeclineTaskResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string TaskActionReason { get; set; }
    }
}
