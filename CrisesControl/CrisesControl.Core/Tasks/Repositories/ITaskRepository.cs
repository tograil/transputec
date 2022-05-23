using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Models;

namespace CrisesControl.Core.Tasks.Repositories;

public interface ITaskRepository
{
    TaskHeaderWithName GeTaskHeader(int incidentId);
    TaskHeader? GeTaskHeader(int incidentId, int taskHeaderId);
    Task<List<TaskDetails>> GetTasks(int incidentId, int incidentTaskId, bool single = false, int companyId = 0, int? taskHeaderId = null, CancellationToken cancellationToken = default);
    List<CheckListOption> GetChkResponseOptions(int companyId, int userId);
    List<CheckListUpsert> GetTaskCheckList(int incidentTaskId, int companyId, int userId);
    Task<int> CreateTaskHeader(int taskHeaderId, int incidentId, int author, DateTimeOffset nextReviewDate, string reviewFrequency,
                        bool sendReminder, bool isActive, decimal rto, decimal rpo, int currentUserId, int companyId, string timeZoneId, CancellationToken cancellationToken);
    Task<int> CreateTask(int incidentTaskId, int taskHeaderId, int incidentId, string taskTitle, string taskDescription, int taskStatus, double escalationDuration, double expectedCompletionTime,
                        bool hasPredecessor, int updateUserId, int companyId, string timeZoneId, CancellationToken cancellationToken);
    Task SaveTaskCheckLists(List<CheckList> checkLists, int incidentTaskId, int updateUserId, int companyId, string timeZoneId, CancellationToken cancellationToken);
    Task IncidentTaskParticipants(int incidentTaskId, int[] actionUsers, int[] actionGroups, int[] escalationUsers, int[] escalationGroups, CancellationToken cancellationToken);
    Task IncidentTaskPredecessors(int incidentTaskId, int[] predecessors, CancellationToken cancellationToken);
    Task ReorderTask(int incidentId, int taskHeaderId, List<TaskSequence> taskSequences, CancellationToken cancellationToken);
    List<CrisesControl.Core.Assets.Assets> GetTaskAsset(int incidentTaskId, int companyId);
    Task SaveTaskAssets(int incidentTaskId, int[] taskAssets, int currentUserId, int companyId, string timeZoneId, CancellationToken cancellationToken);
    Task DeleteTaskAsset(int incidentTaskId, int[] taskAssets, int currentUserId, int companyId, CancellationToken cancellationToken);
    Task DeleteTask(int incidentTaskId, int taskHeaderId, CancellationToken cancellationToken);
    Task CloneTask(int incidentTaskId, int incidentId, int companyId, int userId, string timeZoneId, CancellationToken cancellationToken);
}