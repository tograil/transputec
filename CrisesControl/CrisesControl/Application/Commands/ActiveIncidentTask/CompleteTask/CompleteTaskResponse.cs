using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask
{
    public class CompleteTaskResponse
    {
        public string Message { get; set; }
        public TaskActiveIncident task { get; set; }
    }
}
