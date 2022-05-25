using CrisesControl.Core.Models;
using CrisesControl.Core.Tasks;

namespace CrisesControl.Api.Application.Commands.Tasks.AddNewIncidentTask;

public class AddNewIncidentTaskResponse
{
    public TaskHeader? TaskHeader { get; set; }
    public List<TaskDetails>? TaskList { get; set; }
}