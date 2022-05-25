using CrisesControl.Core.Tasks;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.ChangePredecessor;

public class ChangePredecessorRequest : IRequest<List<TaskDetails>>
{
    public int IncidentTaskId { get; set; }
    public int[] TaskPredecessor { get; set; }
    public int IncidentId { get; set; }
    public int TaskHeaderId { get; set; }
}