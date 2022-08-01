using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit
{
    public class GetTaskAuditRequest:IRequest<GetTaskAuditResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
    }
}
