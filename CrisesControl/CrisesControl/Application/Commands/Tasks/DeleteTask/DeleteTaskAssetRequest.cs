using CrisesControl.Core.Tasks;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.DeleteTask;

public class DeleteTaskRequest : IRequest<List<TaskDetails>>
{
    public int IncidentTaskId { get; set; }
    public int TaskHeaderId { get; set; }
    public int IncidentId { get; set; }
}