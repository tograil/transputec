using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate
{
    public class SendTaskUpdateResponse
    {
        public string Message { get; set; }
        public TaskActiveIncident Data { get; set; }
    }
}
