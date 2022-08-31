using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Support.ActiveIncidentTasks
{
    public class ActiveIncidentTasksResponse
    {
        public TaskIncidentHeader Task { get; set; }
        public List<IncidentTaskDetails> IncidentTaskDetails { get; set; }
    }
}
