using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetIncidentTasksAudit
{
    public class GetIncidentTasksAuditRequest:IRequest<GetIncidentTasksAuditResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public int CompanyId { get; set; }
    }
}
