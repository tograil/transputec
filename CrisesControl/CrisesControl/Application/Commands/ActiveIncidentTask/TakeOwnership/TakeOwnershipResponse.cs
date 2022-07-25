using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership
{
    public class TakeOwnershipResponse
    {
        public string Message { get; set; }
        public TaskActiveIncident Data { get; set; }
    }
}
