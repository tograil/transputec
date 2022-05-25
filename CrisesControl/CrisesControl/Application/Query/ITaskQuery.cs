﻿using CrisesControl.Api.Application.Query.Common;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Paging;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query
{
    public interface ITaskQuery
    {
        //List<IncidentList> GetAllCompanyIncident(int userId);
        Task<List<TaskDetails>> GetIncidentTask(int incidentId, int incidentTaskId, bool single, CancellationToken cancellationToken);
        Task<List<TaskDetails>> GetTaskDetail(int incidentId, int incidentTaskId, int taskHeaderId, CancellationToken cancellationToken);
        List<CrisesControl.Core.Assets.Assets> GetTaskAsset(int incidentTaskId, int companyId);
        List<CheckListOption> GetChkResponseOptions();
        List<CheckListUpsert> GetTaskCheckList(int incidentTaskId);
    }
}