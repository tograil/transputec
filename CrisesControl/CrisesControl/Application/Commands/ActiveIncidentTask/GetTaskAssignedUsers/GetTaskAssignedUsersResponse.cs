using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAssignedUsers
{
    public class GetTaskAssignedUsersResponse
    {
        public List<TaskAssignedUser> Data { get; set; }
        public string Message { get; set; }
    }
}
