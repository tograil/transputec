using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAssignedUsers
{
    public class GetTaskAssignedUsersRequest:IRequest<GetTaskAssignedUsersResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string TypeName { get; set; }
    }
}
