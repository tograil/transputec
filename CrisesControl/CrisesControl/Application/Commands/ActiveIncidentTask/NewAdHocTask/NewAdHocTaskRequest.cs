using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.NewAdHocTask
{
    public class NewAdHocTaskRequest:IRequest<NewAdHocTaskResponse>
    {
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public double EscalationDuration { get; set; }
        public double ExpectedCompletionTime { get; set; }
        public int[] ActionUsers { get; set; }
        public int[] ActionGroups { get; set; }
        public int[] EscalationUsers { get; set; }
        public int[] EscalationGroups { get; set; }
        public int ActiveIncidentID { get; set; }
    }
}
