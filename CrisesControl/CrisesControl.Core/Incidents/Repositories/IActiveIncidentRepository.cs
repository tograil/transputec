using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Models;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents.Repositories;

public interface IActiveIncidentRepository
{
    Task ProcessKeyHolders(int companyId, int incidentId, int activeIncidentId, int currentUserId,
        int[] incidentKeyHolders);

    Task ProcessImpactedLocation(int[] locationIds, int incidentActivationId, int companyId, string action);

    Task ProcessAffectedLocation(ICollection<AffectedLocation> affectedLocations, int incidentActivationId,
        int companyId, string type = "AFFECTED", string action = "INITIATE");

    Task<ICollection<IncidentActivation>> GetIncidentActivationList(int incidentActivationId, int companyId);

    Task CreateActiveKeyContact(int incidentActivationId, int incidentId,
        IncidentKeyHldLst[] keyHldLst, int currentUserId, int companyId, string timeZoneId);

    Task<int> CreateActiveIncidentTask(int activeIncidentTaskId, int activeIncidentId, int incidentTaskId,
        string taskTitle,
        string taskDescription, bool hasPredecessor, double escalationDuration, double expectedCompletionTime,
        int taskSequence,
        int taskOwnerId, DateTime taskAcceptedDate, DateTime taskEscalatedDate, int taskStatus, int taskCompletedBy,
        int nextIncidentTaskId,
        int previousIncidentTaskId, int previousOwnerId, DateTimeOffset taskActivationDate, int currentUserId,
        int companyId);

    void CreateActiveCheckList(int activeIncidentTaskId, int incidentTaskId, int userId, string timeZoneId = "GMT Standard Time");

    void CreateTaskRecipient(int activeIncidentId, int activeIncidentTaskId, int incidentTaskId);

    List<UpdateIncidentStatusReturn> GetCompanyActiveIncident(int companyId, int userId, string status, int recordStart = 0, int recordLength = 100, string searchString = "", string orderBy = "Name", string orderDir = "asc");
    Task<List<UserTaskHead>> GetUserTasks(int currentUserId);
    Task<TaskIncidentHeader> TaskIncidentHeader(int activeIncidentId);
    Task<TaskIncidentHeader> GetActiveIncidentWorkflow(int activeIncidentID);
    Task<List<IncidentTaskDetails>> _active_incident_tasks(int activeIncidentId, int activeIncidentTaskId);
    Task FixActiveTaskOrder(int activeIncidentId);
    Task<List<TaskPredecessorList>> _active_incident_tasks_successor(int activeIncidentTaskId, int activeIncidentId);
    Task<List<TaskPredecessorList>> _active_incident_tasks_predeccessor(int activeIncidentTaskId, int activeIncidentId);
    Task<List<ActiveTaskParticiants>> _active_participants(int activeIncidentId, int activeTaskId = 0, string recipientType = "");
    Task<List<IncidentTaskDetails>> GetActiveIncidentTasks(int activeIncidentId, int activeIncidentTaskId, int companyId, bool single = false);
    Task<TaskActiveIncident> GetTaskActiveIncidentById(int activeIncidentTaskId, int companyId);
    Task change_participant_type(int userId, int activeIncidentTaskId, int newTypeId, string actionStatus, bool addNew = false);
    Task create_active_participant_list(int objectId, int activeIncidentTaskId, int paticipentTypeId, string objtype, string actionStatus = "UNALLOCATED");
    Task<TaskActiveIncidentParticipant> GetTaskActiveIncidentParticipantById(int activeIncidentTaskId, int currentUserId);
    Task remove_old_delegates(int activeIncidentTaskId);
    Task<int> UpdateTaskActiveIncident(TaskActiveIncident task);
    Task<int> GetTaskActiveIncidentParticipantIdByStatus(int activeIncidentTaskId);
    void notify_users(int ActiveIncidentID, int ActiveIncidentTaskID, List<NotificationUserList> userToNotify, string message, int currentUserId,
        int companyId, string timeZoneId, bool includeKeyContact = true, int source = 1, int[] messageMetod = null, int cascadePlanId = 0);
    Task<int> AddTaskAction(int ActiveIncidentTaskID, string ActionDescription, int currentUserId, int TaskActionTypeID, string TimeZoneId);
    void send_notifiation_to_groups(List<string> groupType, int activeIncidentId, int activeIncidentTaskId,
           string message, int currentUserId, int companyId, string timeZoneId, bool includeKeyContact = true,
           int source = 1, int[] messageMethod = null, List<NotificationUserList> userToNotify = null, int cascadePlanId = 0, string sourceAction = "");
    Task<List<TaskIncidentAction>> GetTaskIncidentActionByDeline(int activeIncidentTaskId);
    Task<bool> check_for_last_member(int activeIncidentId, int activeIncidentTaskId, string group);
   
    Task<List<NotificationUserList>> GetActiveParticipantList(int incidentActivationId, string groupType = "ACTION", int activeIncidentTaskId = 0, bool isTaskRecipient = true);
    Task<TaskActiveIncident> ReallocateTask(int activeIncidentTaskId, string taskActionReason, int reallocateTo, int[] messageMethod, int cascadePlanId, int currentUserId, int companyId, string timeZoneId);
    Task CreatePredecessorJobs(int activeIncidentId, int incidentTaskId, int currentUserId, string timeZoneId);
    Task<TaskActiveIncident> DelegateTask(int activeIncidentTaskId, string taskActionReason, int[] delegateTo, int[] messageMethod, int cascadePlanId, int currentUserId, int companyId, string timeZoneId);
    Task<List<TaskAssignedUser>> GetTaskAssignedUsers(int activeIncidentTaskId, string typeName, int companyId);
    Task<List<ActiveCheckList>> GetActiveTaskCheckList(int activeIncidentTaskId, int companyId, int userId);
    Task<List<TaskAudit>> GetTaskAudit(int activeIncidentTaskId);
    Task<dynamic> GetTaskDetails(int activeIncidentTaskId, int companyId);
    Task<List<IncidentTaskAudit>> GetIncidentTasksAudit(int activeIncidentId, int companyId);
    Task<List<FailedTaskList>> get_unattended_tasks(int companyId, int userId, int activeIncidentId);
    Task<List<GetAllUser>> GetTaskUserList(int start, int length, Search search, string typeName, int activeIncidentTaskId, string companyKey, int outLoginUserId, int outUserCompanyId);
    Task<Incidents> GetIncidentActivation(int activeIncidentId);
    Task<TaskActiveIncident> ReassignTask(int activeIncidentTaskId, int[] actionUsers, int[] escalationUsers,
        string taskActionReason, bool removeCurrentOwner, int currentUserId, int companyId, string timeZoneId);
    Task<int> NewAdHocTask(int activeIncidentId, string taskTitle, string taskDescription, int[] actionUsers, int[] actionGroups, int[] escalationUsers, int[] escalationGroups, double escalationDuration, double expectedCompletionTime, int companyId, int userId, string itmeZoneId);
    Task AdHocIncidentTaskParticipants(int activeIncidentId, int activeIncidentTaskId, int[] actionUsers, int[] actionGroups, int[] escalationUsers, int[] escalationGroups);
    Task adhoc_create_participant_list(int[] uList, int activeIncidentTaskId, int paticipentTypeId, string objtype);
    Task<string> GetCompanyParameter(string key, int companyId, string @default = "",string customerId = "");
    Task<string> LookupWithKey(string key, string Default = "");
    Task<bool> SaveActiveCheckListResponse(int activeIncidentTaskId, List<CheckListOption> checkListResponse, int userId, int companyId, string timeZoneId);
    Task CompleteAllTask(int activeIncidentId, int currentUserId, int companyId, string timeZoneId);

    Task<User?> GetUserById(int id);
    Task<dynamic> incident_tasks_list(int activeIncidentId, int currentUserId, int companyId);
    Task<bool> AddTaskAttachment(int activeIncidentTaskId, string attachmentTitle, string fileName, string sourceFileName, double fileSize, int userId, string timeZoneId);
    Task<List<CheckListHistoryRsp>> GetTaskCheckListHistory(int activeCheckListId, int companyId, int userId);
    Task<List<TaskAssetList>> GetActiveTaskAsset(int activeTaskId, int companyId, int userId);
    void SaveActiveTaskAssets(int activeTaskId, int[] taskAssets, int companyId, int userId);
}