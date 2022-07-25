using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask
{
    public class UnattendedTaskRequest:IRequest<UnattendedTaskResponse>
    {
        public int OutUserCompanyId { get; set; }
        public int OutLoginUserId { get; set; }
        public int ActiveIncidentID { get; set; }
    }
}
