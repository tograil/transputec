using System.Data.SqlTypes;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.Repositories;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.SharedKernel.Utils;
using MediatR;
using Ardalis.GuardClauses;

namespace CrisesControl.Api.Application.Commands.Tasks.CreateIncidentTaskHeader;

public class CreateIncidentTaskHeaderHandler
    : IRequestHandler<CreateIncidentTaskHeaderRequest, CreateIncidentTaskHeaderResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ICompanyRepository _companyRepository;
    private readonly string _timeZoneId = "GMT Standard Time";

    public CreateIncidentTaskHeaderHandler(ITaskRepository TaskRepository,
        ICurrentUser currentUser,
        ICompanyRepository companyRepository)
    {
        _taskRepository = TaskRepository;
        _currentUser = currentUser;
        _companyRepository = companyRepository;
    }

    public async Task<CreateIncidentTaskHeaderResponse> Handle(CreateIncidentTaskHeaderRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateIncidentTaskHeaderRequest));
        int taskHeaderId = await _taskRepository.CreateTaskHeader(request.TaskHeaderId, request.IncidentId, request.Author, request.NextReviewDate, request.ReviewFrequency,
                        request.SendReminder, request.IsActive, request.Rto, request.Rpo, _currentUser.UserId, _currentUser.CompanyId, _timeZoneId, cancellationToken);

        if (taskHeaderId > 0)
        {
            var taskHeader = _taskRepository.GeTaskHeader(request.IncidentId);
            if (taskHeader != null)
            {
                var taskList = await _taskRepository.GetTasks(request.IncidentId, 0, false, _currentUser.CompanyId, taskHeaderId, cancellationToken);
                return new CreateIncidentTaskHeaderResponse() { TaskHeader = taskHeader, TaskList = taskList };
            }
            else
            {
                return new CreateIncidentTaskHeaderResponse();
            }
        }
        else
        {
            return new CreateIncidentTaskHeaderResponse();
        }
    }
}