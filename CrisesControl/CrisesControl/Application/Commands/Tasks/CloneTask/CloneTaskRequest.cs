using CrisesControl.Core.Tasks;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.AddNewIncidentTask;

public class CloneTaskRequest : IRequest<List<TaskDetails>>
{
    public int IncidentTaskId { get; set; }
    public int IncidentId { get; set; }
    public int TaskHeaderId { get; set; }
}