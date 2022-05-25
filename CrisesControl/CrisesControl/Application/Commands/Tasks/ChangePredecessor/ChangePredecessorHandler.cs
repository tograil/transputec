using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.ChangePredecessor;

public class ChangePredecessorHandler
    : IRequestHandler<ChangePredecessorRequest, List<TaskDetails>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;

    public ChangePredecessorHandler(ITaskRepository taskRepository, ICurrentUser currentUser)
    {
        _taskRepository = taskRepository;
        _currentUser = currentUser;
    }

    public async Task<List<TaskDetails>> Handle(ChangePredecessorRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(ChangePredecessorRequest));
        await _taskRepository.IncidentTaskPredecessors(request.IncidentTaskId, request.TaskPredecessor, cancellationToken);
        var taskList = await _taskRepository.GetTasks(request.IncidentId, 0, false, _currentUser.CompanyId, request.TaskHeaderId, cancellationToken);
        return taskList ?? new List<TaskDetails>();
    }
}