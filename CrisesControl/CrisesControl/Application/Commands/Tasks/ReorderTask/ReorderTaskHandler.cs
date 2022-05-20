using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.ReorderTask;

public class ReorderTaskHandler
    : IRequestHandler<ReorderTaskRequest, List<TaskDetails>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;

    public ReorderTaskHandler(ITaskRepository taskRepository, ICurrentUser currentUser)
    {
        _taskRepository = taskRepository;
        _currentUser = currentUser;
    }

    public async Task<List<TaskDetails>> Handle(ReorderTaskRequest request, CancellationToken cancellationToken)
    {
        await _taskRepository.ReorderTask(request.IncidentId, request.TaskHeaderId, request.TaskSequences, cancellationToken);
        var taskList = await _taskRepository.GetTasks(request.IncidentId, 0, false, _currentUser.CompanyId, request.TaskHeaderId, cancellationToken);
        return taskList ?? new List<TaskDetails>();
    }
}