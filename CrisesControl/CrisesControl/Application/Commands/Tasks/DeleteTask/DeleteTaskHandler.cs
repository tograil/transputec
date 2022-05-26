using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.DeleteTask;

public class DeleteTaskHandler
    : IRequestHandler<DeleteTaskRequest, List<TaskDetails>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteTaskHandler(ITaskRepository taskRepository, ICurrentUser currentUser)
    {
        _taskRepository = taskRepository;
        _currentUser = currentUser;
    }

    public async Task<List<TaskDetails>> Handle(DeleteTaskRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(DeleteTaskRequest));
        await _taskRepository.DeleteTask(request.IncidentTaskId, request.TaskHeaderId, cancellationToken);
        var taskList = await _taskRepository.GetTasks(request.IncidentId, 0, false, _currentUser.CompanyId, request.TaskHeaderId, cancellationToken);
        return taskList ?? new List<TaskDetails>();
    }
}