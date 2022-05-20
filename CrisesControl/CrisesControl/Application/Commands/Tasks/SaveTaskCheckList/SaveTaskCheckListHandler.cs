using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.SaveTaskCheckList;

public class SaveTaskCheckListHandler
    : IRequestHandler<SaveTaskCheckListRequest, bool>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;
    private readonly string _timeZoneId = "GMT Standard Time";

    public SaveTaskCheckListHandler(ITaskRepository TaskRepository, ICurrentUser currentUser)
    {
        _taskRepository = TaskRepository;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(SaveTaskCheckListRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(SaveTaskCheckListRequest));
        await _taskRepository.SaveTaskCheckLists(request.CheckListItems, request.IncidentTaskId, _currentUser.UserId, _currentUser.CompanyId, _timeZoneId, cancellationToken);
        return true;
    }
}