using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask
{
    public class AcceptTaskResponse
    {
        public string Message { get; set; }
        public TaskActiveIncident result { get; set; }
    }
}
