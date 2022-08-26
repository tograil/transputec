using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTaskList
{
    public class GetUserTaskListRequest : IRequest<GetUserTaskListResponse>
    {
        public int ActiveIncidentId { get; set; }
        public int CurrentUserId { get; set; }
        public int CompanyId { get; set; }
    }
}
