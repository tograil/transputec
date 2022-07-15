using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask
{
    public class CompleteTaskRequest:IRequest<CompleteTaskResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string TaskActionReason { get; set; }
        public string TaskCompletionNote { get; set; }
        public string[] SendUpdateTo { get; set; }
        public int[] MessageMethod { get; set; }
        public int CascadePlanID { get; set; }
    }
}
