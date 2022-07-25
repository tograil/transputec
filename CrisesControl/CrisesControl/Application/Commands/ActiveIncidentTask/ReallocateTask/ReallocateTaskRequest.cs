using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask
{
    public class ReallocateTaskRequest:IRequest<ReallocateTaskResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string TaskActionReason { get; set; }
        public int ReallocateTo { get; set; }
        public int[] MessageMethod { get; set; }
        public int CascadePlanID { get; set; }
    }
}
