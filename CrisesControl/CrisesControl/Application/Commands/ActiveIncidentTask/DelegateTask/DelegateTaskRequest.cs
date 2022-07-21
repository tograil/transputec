using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask
{
    public class DelegateTaskRequest:IRequest<DelegateTaskResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string TaskActionReason { get; set; }
        public int[] DelegateTo { get; set; }
        public int[] MessageMethod { get; set; }
        public int CascadePlanID { get; set; }
    }
}
