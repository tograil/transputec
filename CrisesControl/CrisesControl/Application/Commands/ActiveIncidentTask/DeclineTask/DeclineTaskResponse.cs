using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask
{
    public class DeclineTaskResponse
    {
        public string Message { get; set; }
        public TaskActiveIncident result { get; set; }
    }
}
