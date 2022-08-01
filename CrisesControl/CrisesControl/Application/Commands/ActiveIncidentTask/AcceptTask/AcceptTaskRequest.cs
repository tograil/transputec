using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask
{
    public class AcceptTaskRequest:IRequest<AcceptTaskResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
    }
}
