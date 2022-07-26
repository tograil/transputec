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

    Task CreateActiveCheckList(int activeIncidentTaskId, int incidentTaskId, int userId, string timeZoneId = "GMT Standard Time");

    Task CreateTaskRecipient(int activeIncidentId, int activeIncidentTaskId, int incidentTaskId);

    List<UpdateIncidentStatusReturn> GetCompanyActiveIncident(int CompanyID, int UserID, string Status, int RecordStart = 0, int RecordLength = 100, string SearchString = "", string OrderBy = "Name", string OrderDir = "asc");
    Task<List<UserTaskHead>> GetUserTasks(int CurrentUserID);
    Task<TaskIncidentHeader> TaskIncidentHeader(int ActiveIncidentID);
    Task<TaskIncidentHeader> GetActiveIncidentWorkflow(int activeIncidentID);
    Task<List<IncidentTaskDetails>> _active_incident_tasks(int ActiveIncidentID, int ActiveIncidentTaskID);
    Task FixActiveTaskOrder(int ActiveIncidentID);
    Task<List<TaskPredecessorList>> _active_incident_tasks_successor(int ActiveIncidentTaskID, int ActiveIncidentID);
    Task<List<TaskPredecessorList>> _active_incident_tasks_predeccessor(int ActiveIncidentTaskID, int ActiveIncidentID);
    Task<List<ActiveTaskParticiants>> _active_participants(int ActiveIncidentID, int ActiveTaskID = 0, string RecipientType = "");
    Task<List<IncidentTaskDetails>> GetActiveIncidentTasks(int ActiveIncidentID, int ActiveIncidentTaskID, int CompanyID, bool single = false);
    Task<TaskActiveIncident> GetTaskActiveIncidentById(int ActiveIncidentTaskID, int CompanyID);
    Task change_participant_type(int UserID, int ActiveIncidentTaskID, int NewTypeId, string ActionStatus, bool AddNew = false);
    Task create_active_participant_list(int ObjectId, int ActiveIncidentTaskID, int PaticipentTypeId, string objtype, string ActionStatus = "UNALLOCATED");
    Task<TaskActiveIncidentParticipant> GetTaskActiveIncidentParticipantById(int ActiveIncidentTaskID, int CurrentUserID);
    Task remove_old_delegates(int ActiveIncidentTaskID);
    Task<int> UpdateTaskActiveIncident(TaskActiveIncident task);
    Task<int> GetTaskActiveIncidentParticipantIdByStatus(int ActiveIncidentTaskID);
    Task notify_users(int ActiveIncidentID, int ActiveIncidentTaskID, List<NotificationUserList> UserToNotify, string Message, int CurrentUserID,
        int CompanyID, string TimeZoneId, bool IncludeKeyContact = true, int Source = 1, int[] MessageMetod = null, int CascadePlanID = 0);
    Task<int> AddTaskAction(int ActiveIncidentTaskID, string ActionDescription, int CurrentUserID, int TaskActionTypeID, string TimeZoneId);
    Task send_notifiation_to_groups(List<string> GroupType, int ActiveIncidentID, int ActiveIncidentTaskID,
           string Message, int CurrentUserID, int CompanyID, string TimeZoneId, bool IncludeKeyContact = true,
           int Source = 1, int[] MessageMethod = null, List<NotificationUserList> UserToNotify = null, int CascadePlanID = 0, string sourceAction = "");
    Task<List<TaskIncidentAction>> GetTaskIncidentActionByDeline(int ActiveIncidentTaskID);
    Task<bool> check_for_last_member(int ActiveIncidentID, int ActiveIncidentTaskID, string Group);
   
    Task<List<NotificationUserList>> GetActiveParticipantList(int IncidentActivationId, string GroupType = "ACTION", int ActiveIncidentTaskID = 0, bool IsTaskRecipient = true);
    Task<TaskActiveIncident> ReallocateTask(int ActiveIncidentTaskID, string TaskActionReason, int ReallocateTo, int[] MessageMethod, int CascadePlanID, int CurrentUserID, int CompanyID, string TimeZoneId);
    Task CreatePredecessorJobs(int ActiveIncidentID, int IncidentTaskID, int CurrentUserID, string TimeZoneId);
    Task<TaskActiveIncident> DelegateTask(int ActiveIncidentTaskID, string TaskActionReason, int[] DelegateTo, int[] MessageMethod, int CascadePlanID, int CurrentUserID, int CompanyID, string TimeZoneId);
    Task<List<TaskAssignedUser>> GetTaskAssignedUsers(int ActiveIncidentTaskID, string TypeName, int CompanyID);
    Task<List<ActiveCheckList>> GetActiveTaskCheckList(int ActiveIncidentTaskID, int CompanyID, int UserID);
    Task<List<TaskAudit>> GetTaskAudit(int ActiveIncidentTaskID);
    Task<dynamic> GetTaskDetails(int ActiveIncidentTaskID, int CompanyID);
    Task<List<IncidentTaskAudit>> GetIncidentTasksAudit(int ActiveIncidentID, int CompanyID);
    Task<List<FailedTaskList>> get_unattended_tasks(int CompanyID, int UserID, int ActiveIncidentID);
    Task<List<GetAllUser>> GetTaskUserList(int Start, int Length, Search Search, string TypeName, int ActiveIncidentTaskID, string CompanyKey, int OutLoginUserId, int OutUserCompanyId);
    Task<Incidents> GetIncidentActivation(int ActiveIncidentID);
    Task<TaskActiveIncident> ReassignTask(int ActiveIncidentTaskID, int[] ActionUsers, int[] EscalationUsers,
        string TaskActionReason, bool RemoveCurrentOwner, int CurrentUserID, int CompanyID, string TimeZoneId);
    Task<int> NewAdHocTask(int ActiveIncidentID, string TaskTitle, string TaskDescription, int[] ActionUsers, int[] ActionGroups, int[] EscalationUsers, int[] EscalationGroups, double EscalationDuration, double ExpectedCompletionTime, int CompanyID, int UserID, string TimeZoneId);
    Task AdHocIncidentTaskParticipants(int ActiveIncidentID, int ActiveIncidentTaskID, int[] ActionUsers, int[] ActionGroups, int[] EscalationUsers, int[] EscalationGroups);
    Task adhoc_create_participant_list(int[] UList, int ActiveIncidentTaskID, int PaticipentTypeId, string objtype);
    Task<string> GetCompanyParameter(string key, int companyId, string @default = "",string customerId = "");
    Task<string> LookupWithKey(string Key, string Default = "");
    Task<bool> SaveActiveCheckListResponse(int ActiveIncidentTaskID, List<CheckListOption> CheckListResponse, int UserID, int CompanyID, string TimeZoneId);
    Task CompleteAllTask(int ActiveIncidentID, int CurrentUserID, int CompanyID, string TimeZoneId);

    Task<User?> GetUserById(int id);

}