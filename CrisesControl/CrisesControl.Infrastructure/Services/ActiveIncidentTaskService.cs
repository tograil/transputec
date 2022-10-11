using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Services;

public class ActiveIncidentTaskService : IActiveIncidentTaskService
{
    private readonly CrisesControlContext _context;
    private readonly IActiveIncidentRepository _activeIncidentRepository;

    public ActiveIncidentTaskService(CrisesControlContext context,
        IActiveIncidentRepository activeIncidentRepository)
    {
        _context = context;
        _activeIncidentRepository = activeIncidentRepository;
    }

    public async Task StartTaskAllocation(int incidentId, int activeIncidentId, int currentUserId, int companyId)
    {
        var currentTask = await _context.Set<TaskHeader>()
            .FirstOrDefaultAsync(x => x.IncidentId == incidentId);

        if (currentTask is not null && currentTask.IsActive)
        {
            var taskList = await _context.Set<TaskIncident>()
                .Where(x => x.IncidentId == incidentId 
                            && x.TaskHeaderId == currentTask.TaskHeaderId 
                            && x.Status == 1)
                .ToListAsync();

            foreach (var taskIncident in taskList)
            {
                var activationDate = DateTime.Now.GetDateTimeOffset();
                var activeIncidentTaskId = await _activeIncidentRepository.CreateActiveIncidentTask(0, activeIncidentId,
                    taskIncident.IncidentTaskId, taskIncident.TaskTitle!,
                    taskIncident.TaskDescription!, taskIncident.HasPredecessor, taskIncident.EscalationDuration,
                    taskIncident.ExpectedCompletionTime, taskIncident.TaskSequence,
                    0, (DateTime) SqlDateTime.Null, (DateTime) SqlDateTime.Null, 1, 0, 0, 0, 0, activationDate,
                    currentUserId, companyId);

                if (activeIncidentTaskId > 0)
                {
                    _activeIncidentRepository.CreateActiveCheckList(activeIncidentTaskId,
                        taskIncident.IncidentTaskId, currentUserId);

                     _activeIncidentRepository.CreateTaskRecipient(activeIncidentId, activeIncidentTaskId,
                        taskIncident.IncidentTaskId);

                    var activePdList = await _context.Set<TaskIncidentPredecessor>()
                        .Where(x => x.IncidentTaskId == taskIncident.IncidentTaskId)
                        .Select(x => x.PredecessorTaskId).ToListAsync();

                    if (activePdList.Any())
                    {
                        await ActiveIncidentTaskPredecessors(activeIncidentTaskId, activePdList);
                    }
                }
            }

        }
    }

    public async Task CopyAssets(int incidentId, int activeIncidentId, int messageId)
    {
        var pIncident = new SqlParameter("@IncidentID", incidentId);
        var pActiveIncidentId = new SqlParameter("@ActiveIncidentID", activeIncidentId);

        await _context.Database.ExecuteSqlRawAsync("Pro_Create_Active_Assets @IncidentID,@ActiveIncidentID",
            pIncident, pActiveIncidentId);

        var pMessageId = new SqlParameter("@MessageID", messageId);
        var pIncidentActivationId = new SqlParameter("@IncidentActivationID", activeIncidentId);
        var pCustomerTime = new SqlParameter("@CustomerTime", DateTime.Now.GetDateTimeOffset());

        await _context.Database.ExecuteSqlRawAsync(
            "Pro_Create_Launch_Incident_Message_List @IncidentActivationID,@MessageID,@CustomerTime"
            , pIncidentActivationId, pMessageId, pCustomerTime);
    }

    private async Task ActiveIncidentTaskPredecessors(int activeIncidentTaskId, ICollection<int> predecessors)
    {
        var oldPdList = await _context.Set<TaskActiveIncidentPredecessor>()
            .Where(x => x.ActiveIncidentTaskId == activeIncidentTaskId).ToListAsync();

        _context.RemoveRange(oldPdList);
        await _context.SaveChangesAsync();

        foreach (var predecessor in predecessors)
        {
            var itp = new TaskActiveIncidentPredecessor
            {
                ActiveIncidentTaskId = activeIncidentTaskId,
                PredecessorTaskId = predecessor,
                Status = "CREATED"
            };
            await _context.AddAsync(itp);
        }

        await _context.SaveChangesAsync();
    }
}