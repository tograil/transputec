using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddAttachment;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddNotes;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskAsset;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskCheckList;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetIncidentTasksAudit;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAssignedUsers;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskCheckListHistory;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskDetails;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.NewAdHocTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReassignTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveCheckListResponse;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask;

namespace CrisesControl.Api.Application.Query
{
    public interface IActiveIncidentQuery
    {
        Task<GetUserTaskResponse> GetUserTasks(GetUserTaskRequest request);
        Task<ActiveIncidentTasksResponse> ActiveIncidentTasks(ActiveIncidentTasksRequest request);
        Task<AcceptTaskResponse> AcceptTask(AcceptTaskRequest request);
        Task<DeclineTaskResponse> DeclineTask(DeclineTaskRequest request);
        Task<CompleteTaskResponse> CompleteTask(CompleteTaskRequest request);
        Task<DelegateTaskResponse> DelegateTask(DelegateTaskRequest request);
        Task<ReallocateTaskResponse> ReallocateTask(ReallocateTaskRequest request);
        Task<GetTaskAssignedUsersResponse> GetTaskAssignedUsers(GetTaskAssignedUsersRequest request);
        Task<GetActiveTaskCheckListResponse> GetActiveTaskCheckList(GetActiveTaskCheckListRequest request);
        Task<SendTaskUpdateResponse> SendTaskUpdate(SendTaskUpdateRequest request);
        Task<GetTaskAuditResponse> GetTaskAudit(GetTaskAuditRequest request);
        Task<TakeOwnershipResponse> TakeOwnership(TakeOwnershipRequest request);
        Task<GetTaskDetailsResponse> GetTaskDetails(GetTaskDetailsRequest request);
        Task<GetTaskUserListResponse> GetTaskUserList(GetTaskUserListRequest request);
        Task<GetIncidentTasksAuditResponse> GetIncidentTasksAudit(GetIncidentTasksAuditRequest request);
        Task<UnattendedTaskResponse> UnattendedTask(UnattendedTaskRequest request);
        Task<ReassignTaskResponse> ReassignTask(ReassignTaskRequest request);
        Task<NewAdHocTaskResponse> NewAdHocTask(NewAdHocTaskRequest request);
        Task<AddNotesResponse> AddNotes(AddNotesRequest request);
        Task<SaveActiveCheckListResponse> SaveActiveCheckListResponse(SaveActiveCheckListResponseRequest request);
        Task<SaveActiveTaskAssetResponse> SaveActiveTaskAsset(SaveActiveTaskAssetRequest request);
        Task<GetActiveTaskAssetResponse> GetActiveTaskAsset(GetActiveTaskAssetRequest request);
        Task<GetTaskCheckListHistoryResponse> GetTaskCheckListHistory(GetTaskCheckListHistoryRequest request);
        Task<AddAttachmentResponse> AddAttachment(AddAttachmentRequest request);
    }
}
