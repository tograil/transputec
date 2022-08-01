using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask
{
    public class GetUserTaskResponse
    {
        public List<UserTaskHead> Data { get; set; }
        public string Message { get; set; }
    }
}
