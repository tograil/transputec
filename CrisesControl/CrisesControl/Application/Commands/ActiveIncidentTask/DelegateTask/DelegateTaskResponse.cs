using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask
{
    public class DelegateTaskResponse
    {
        public TaskActiveIncident Data { get; set; }
        public string Message { get; set; }
    }
}
