using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask
{
    public class ReallocateTaskResponse
    {
        public TaskActiveIncident Data { get; set; }
        public string Message { get; set; }
    }
}
