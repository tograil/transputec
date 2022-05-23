using CrisesControl.Core.Tasks;

namespace CrisesControl.Api.Application.Commands.Tasks.CreateIncidentTaskHeader;

public class CreateIncidentTaskHeaderResponse
{
    public TaskHeaderWithName? TaskHeader { get; set; }
    public List<TaskDetails>? TaskList { get; set; }
}