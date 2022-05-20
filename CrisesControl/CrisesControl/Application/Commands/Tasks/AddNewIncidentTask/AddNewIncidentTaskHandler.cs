using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.AddNewIncidentTask;

public class AddNewIncidentTaskHandler
    : IRequestHandler<AddNewIncidentTaskRequest, AddNewIncidentTaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ICompanyRepository _companyRepository;
    private readonly string _timeZoneId = "GMT Standard Time";

    public AddNewIncidentTaskHandler(ITaskRepository taskRepository,
        ICurrentUser currentUser,
        ICompanyRepository companyRepository)
    {
        _taskRepository = taskRepository;
        _currentUser = currentUser;
        _companyRepository = companyRepository;
    }

    public async Task<AddNewIncidentTaskResponse> Handle(AddNewIncidentTaskRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(AddNewIncidentTaskRequest));
        //Create Incident Task
        int taskId = await _taskRepository.CreateTask(request.IncidentTaskId, request.TaskHeaderId, request.IncidentId, request.TaskTitle, request.TaskDescription,
                        request.Status, request.EscalationDuration, request.ExpectedCompletionTime, request.HasPredecessor, _currentUser.UserId, _currentUser.CompanyId, _timeZoneId, cancellationToken);

        if (taskId > 0)
        {
            await _taskRepository.SaveTaskCheckLists(request.CheckListItems, taskId, _currentUser.UserId, _currentUser.CompanyId, _timeZoneId, cancellationToken);
            await _taskRepository.IncidentTaskParticipants(taskId, request.ActionUsers, request.ActionGroups, request.EscalationUsers, request.EscalationGroups, cancellationToken);

            //Create List of Incident action and escalation groups/users
            if (request.Status == 0)
            {
                var emptylist = new List<int>();
                int[] emptyarray = emptylist.ToArray();
                request.TaskPredecessor = emptyarray;

                await _taskRepository.IncidentTaskPredecessors(request.IncidentTaskId, request.TaskPredecessor, cancellationToken);
            }

            await _taskRepository.IncidentTaskPredecessors(taskId, request.TaskPredecessor, cancellationToken);
        }

        var taskHeader = _taskRepository.GeTaskHeader(request.IncidentId, request.TaskHeaderId);
        if (taskHeader != null)
        {

            var taskList = await _taskRepository.GetTasks(taskHeader.IncidentId, 0, false, _currentUser.CompanyId, taskHeader.TaskHeaderId, cancellationToken);
            return new AddNewIncidentTaskResponse() { TaskHeader = taskHeader, TaskList = taskList };
        }
        else
        {
            return new AddNewIncidentTaskResponse();
        }
    }
}