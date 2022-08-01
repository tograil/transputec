using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate
{
    public class SendTaskUpdateRequest:IRequest<SendTaskUpdateResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string TaskActionReason { get; set; }
        public string[] SendUpdateTo { get; set; }
        public int[] MessageMethod { get; set; }
        public List<NotificationUserList> MembersToNotify { get; set; }
        
    }
}
