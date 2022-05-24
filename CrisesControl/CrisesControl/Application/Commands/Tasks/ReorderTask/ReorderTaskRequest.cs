using CrisesControl.Core.Tasks;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.ReorderTask;

public class ReorderTaskRequest : IRequest<List<TaskDetails>>
{
    public List<TaskSequence> TaskSequences { get; set; }
    public int IncidentId { get; set; }
    public int TaskHeaderId { get; set; }
}