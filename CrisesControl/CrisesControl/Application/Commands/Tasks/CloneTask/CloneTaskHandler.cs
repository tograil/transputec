using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.AddNewIncidentTask;

public class CloneTaskHandler
    : IRequestHandler<CloneTaskRequest, List<TaskDetails>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;
    private readonly string _timeZoneId = "GMT Standard Time";

    public CloneTaskHandler(ITaskRepository TaskRepository,
        ICurrentUser currentUser)
    {
        _taskRepository = TaskRepository;
        _currentUser = currentUser;
    }

    public async Task<List<TaskDetails>> Handle(CloneTaskRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CloneTaskRequest));
        await _taskRepository.CloneTask(request.IncidentTaskId, request.IncidentId, _currentUser.CompanyId, _currentUser.UserId, _timeZoneId, cancellationToken);
        var taskList = await _taskRepository.GetTasks(request.IncidentId, 0, false, _currentUser.CompanyId, request.TaskHeaderId, cancellationToken);
        return taskList ?? new List<TaskDetails>();
    }
}