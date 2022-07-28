using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReassignTask
{
    public class ReassignTaskResponse
    {
        public TaskActiveIncident data { get; set; }
        public string Message { get; set; }
    }
}
