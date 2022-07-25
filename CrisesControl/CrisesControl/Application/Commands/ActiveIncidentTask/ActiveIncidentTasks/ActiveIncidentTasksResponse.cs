using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks
{
    public class ActiveIncidentTasksResponse
    {
        public TaskIncidentHeader Data { get; set; }
        public List<IncidentTaskDetails> incidentTasksList { get; set; }
    }
}
