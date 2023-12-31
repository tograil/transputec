﻿using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.Repositories;

namespace CrisesControl.Api.Application.Query;

public class TaskQuery : ITaskQuery
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUser _currentUser;

    public TaskQuery(ITaskRepository taskRepository,
        ICurrentUser currentUser)
    {
        _taskRepository = taskRepository;
        _currentUser = currentUser;
    }

    //public List<IncidentList> GetAllCompanyIncident(int userId)
    //{
    //    return _incidentRepository.GetCompanyIncident(_currentUser.CompanyId, userId > 0 ? userId : _currentUser.UserId);
    //}

    public async Task<List<TaskDetails>> GetIncidentTask(int incidentId, int incidentTaskId, bool single, CancellationToken cancellationToken)
    {
        return await _taskRepository.GetTasks(incidentId, incidentTaskId, single, _currentUser.CompanyId, null, cancellationToken);
    }

    public async Task<List<TaskDetails>> GetTaskDetail(int incidentId, int incidentTaskId, int taskHeaderId, CancellationToken cancellationToken)
    {
        return await _taskRepository.GetTasks(incidentId, incidentTaskId, true, _currentUser.CompanyId, taskHeaderId, cancellationToken);
    }

    public List<CrisesControl.Core.Assets.Assets> GetTaskAsset(int incidentTaskId, int companyId)
    {
        return _taskRepository.GetTaskAsset(incidentTaskId, companyId);
    }

    public List<CheckListOption> GetChkResponseOptions()
    {
        return _taskRepository.GetChkResponseOptions(_currentUser.CompanyId, _currentUser.UserId);
    }

    public List<CheckListUpsert> GetTaskCheckList(int incidentTaskId)
    {
        return _taskRepository.GetTaskCheckList(incidentTaskId, _currentUser.CompanyId, _currentUser.UserId);
    }

}