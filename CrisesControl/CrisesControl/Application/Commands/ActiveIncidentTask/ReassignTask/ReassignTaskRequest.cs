using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReassignTask
{
    public class ReassignTaskRequest:IRequest<ReassignTaskResponse>
    {
        public  int ActiveIncidentTaskID { get; set; }
        public int[] ActionUsers { get; set; }
        public int[] EscalationUsers { get; set; }
        public string TaskActionReason { get; set; }
        public bool RemoveCurrentOwner { get; set; } 

    }
}
