using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask
{
    public class UnattendedTaskResponse
    {
        public List<FailedTaskList> Data { get; set; }
        public string Message { get; set; }
    }
}
