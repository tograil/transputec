using AutoMapper;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddNotes;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskCheckList;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetIncidentTasksAudit;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAssignedUsers;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskDetails;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.NewAdHocTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReassignTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveCheckListResponse;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class ActiveIncidentQuery:IActiveIncidentQuery
    {
        private readonly IActiveIncidentRepository _activeIncidentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ActiveIncidentQuery> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;
        private string MessageSourceAction = string.Empty;
        private readonly IPaging _paging;
        private DBCommon _dBCommon;
        public ActiveIncidentQuery(IActiveIncidentRepository activeIncidentRepository, IUserRepository userRepository, IMapper mapper, ILogger<ActiveIncidentQuery> logger, ICurrentUser currentUser, DBCommon dBCommon, IPaging paging)
        {
            this._activeIncidentRepository = activeIncidentRepository;
            this._mapper = mapper;
            this._logger = logger;
            this._currentUser = currentUser;
            this._userRepository = userRepository;
            this._dBCommon = dBCommon; ;
            this._paging = paging;
        }

        public async Task<AcceptTaskResponse> AcceptTask(AcceptTaskRequest request)
        {
            try
            {
                var response = new AcceptTaskResponse();
                MessageSourceAction = SourceAction.TaskAccepted;
                var task = await _activeIncidentRepository.GetTaskActiveIncidentById(request.ActiveIncidentTaskID, _currentUser.CompanyId);
                if (task != null)
                {
                    if (task.TaskStatus != 1 && task.TaskStatus != 4 && task.TaskStatus != 6)
                    {
                       // response.ErrorId = 184;
                        response.Message = "Cannot accept the task, already accepted or declined by you";
                        return response;
                    }
                    else if ((task.TaskStatus == 2 || (_currentUser.UserId != task.TaskOwnerId && task.TaskOwnerId != 0 && task.TaskStatus != 4)))
                    {
                        if (_currentUser.UserId != task.TaskOwnerId)
                        {
                           // result.ErrorId = 184;
                            response.Message = "Task already accepted";
                        }
                        else
                        {
                           // result.ErrorId = 189;
                            response.Message = "You have already accepted the task";
                        }
                        return response;
                    }

                    int previousOwner = task.TaskOwnerId;
                    string ActionStatus = "UNALLOCATED";
                    

                    if (previousOwner > 0)
                    {
                        var pt_type = await _activeIncidentRepository.GetTaskActiveIncidentParticipantById(request.ActiveIncidentTaskID, _currentUser.UserId);
                        if (pt_type != null)
                        {
                            ActionStatus = pt_type.ActionStatus;
                            await _activeIncidentRepository.change_participant_type(_currentUser.UserId, request.ActiveIncidentTaskID, pt_type.PreviousParticipantTypeId, ActionStatus);
                            await _activeIncidentRepository.remove_old_delegates(request.ActiveIncidentTaskID);
                        }
                    }
                    else
                    {
                        await _activeIncidentRepository.change_participant_type(_currentUser.UserId, request.ActiveIncidentTaskID, 0, "ACCEPTED");
                    }
                    if (ActionStatus != "DELEGATED")
                    {
                        if (task.TaskOwnerId <= 0)
                        {
                            task.TaskAcceptedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                            //DBC.DeleteScheduledJob("START_ESCL_TASK_" + task.ActiveIncidentTaskId, "TASK_SCHEDULE");
                            //CreateTaskAcceptedJob(task.ActiveIncidentTaskId, task.TaskAcceptedDate, task.ExpectedCompletionTime, _currentUser.TimeZone);
                        }
                        task.PreviousOwnerId = previousOwner;
                        task.TaskOwnerId = _currentUser.UserId;
                        task.TaskStatus = 2;

                        task.UpdatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                        task.UpdatedBy = _currentUser.UserId;
                       var ActivetaskId= await _activeIncidentRepository.UpdateTaskActiveIncident(task);
                    }
                    //Get Action By username
                    var action_user =await _userRepository.GetUser(_currentUser.CompanyId,_currentUser.UserId);
                    string username = "";
                    if (action_user != null)
                    {
                        username = action_user.FirstName + " " + action_user.LastName;
                    }

                    string task_action = "Task accepted by " + username;

                    //Add task action history
                    await _activeIncidentRepository.AddTaskAction(request.ActiveIncidentTaskID, task_action, _currentUser.UserId, task.TaskStatus, _currentUser.TimeZone);

                    bool NotifyKeyContact = false;
                    bool.TryParse(await _activeIncidentRepository.GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", _currentUser.CompanyId), out NotifyKeyContact);

                    List<NotificationUserList> TaskPtcpntList = new List<NotificationUserList>();

                    if (ActionStatus == "REALLOCATED" || ActionStatus == "DELEGATED")
                    {
                        TaskPtcpntList.Add(new NotificationUserList( previousOwner, true ));

                        if (ActionStatus == "DELEGATED")
                        {
                            int old_delegate = await _activeIncidentRepository.GetTaskActiveIncidentParticipantIdByStatus(request.ActiveIncidentTaskID);

                            TaskPtcpntList.Add(new NotificationUserList(previousOwner, true));
                        }

                        string action_update = "Reallocated task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate( task.TaskTitle, 50) + "\" is accepted by " + username + ".";
                        await   _activeIncidentRepository.notify_users(task.ActiveIncidentId, task.ActiveIncidentTaskId, TaskPtcpntList, action_update, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone, NotifyKeyContact, 3);
                    }
                    else
                    {
                        List<string> grp = new List<string>();
                        //if(task.TaskEscalatedDate.Year > 2000) {
                        //    grp.Add("ESCALATION");
                        //    grp.Add("ACTION");
                        //} else {
                        //    grp.Add("ACTION");
                        //}
                        string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 50) + "\" is accepted by " + username + ".";
                        await _activeIncidentRepository.send_notifiation_to_groups(grp, task.ActiveIncidentId, task.ActiveIncidentTaskId, action_update, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone, NotifyKeyContact, 3, sourceAction: MessageSourceAction);
                    }

                    response.result = task;
                    return response;
                }
                return new AcceptTaskResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActiveIncidentTasksResponse> ActiveIncidentTasks(ActiveIncidentTasksRequest request)
        {
            try
            {
                var header  = await _activeIncidentRepository.GetActiveIncidentWorkflow(request.ActiveIncidentID);
                var TaskList = await _activeIncidentRepository.GetActiveIncidentTasks(request.ActiveIncidentID, 0, _currentUser.CompanyId, false);
                var result = _mapper.Map<TaskIncidentHeader>(header);
                var response = new ActiveIncidentTasksResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.incidentTasksList = TaskList;


                }
                else
                {
                    response.Data = result;
                    response.incidentTasksList = TaskList;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompleteTaskResponse> CompleteTask(CompleteTaskRequest request)
        {
            try
            {
                var result = new CompleteTaskResponse();
                var task = await _activeIncidentRepository.GetTaskActiveIncidentById(request.ActiveIncidentTaskID, _currentUser.CompanyId);
                if (task != null)
                {

                    if (task.TaskOwnerId != _currentUser.UserId)
                    {
                        
                        result.Message = "You are not longer the own of the task";
                        return result;
                    }
                    else if (task.TaskStatus == 7)
                    {
                        
                        result.Message = "Task already completed! Task cannot be completed.";
                        return result;
                    }

                    task.TaskStatus = 7;
                    task.HasNotes = string.IsNullOrEmpty(request.TaskCompletionNote) ? false : true;
                    task.UpdatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                    task.UpdatedBy = _currentUser.UserId;
                    task.TaskCompletedBy = _currentUser.UserId;
                    await _activeIncidentRepository.UpdateTaskActiveIncident(task);

                    if (!string.IsNullOrEmpty(request.TaskCompletionNote))
                    {
                      await _dBCommon.IncidentNote(task.ActiveIncidentTaskId, "TASK", request.TaskCompletionNote, _currentUser.CompanyId, _currentUser.UserId);
                    }

                    //Get Action By username
                    var action_user = await _userRepository.GetUser(_currentUser.CompanyId, _currentUser.UserId);
                    string username = "";
                    if (action_user != null)
                    {
                        username = action_user.FirstName + " " + action_user.LastName;
                    }

                    string task_action = "Task completed by " + username + "<br/>Comment:" + request.TaskActionReason;

                    //Add task action history
                  await  _activeIncidentRepository.AddTaskAction(request.ActiveIncidentTaskID, task_action, _currentUser.UserId, task.TaskStatus, _currentUser.TimeZone);

                    List<string> grp = new List<string>();

                    if (request.SendUpdateTo.Contains("ACTION_MEMBERS"))
                    {
                        grp.Add("ACTION");
                    }

                    if (request.SendUpdateTo.Contains("INCIDENT_MEMBERS"))
                    {
                        grp.Add("INCIDENT");
                    }

                    if (request.SendUpdateTo.Contains("ESCALATION_MEMBERS"))
                    {
                        grp.Add("ESCALATION");
                    }

                    //Get list of existing participant of the task
                    bool NotifyKeyContact = false;
                    if (request.SendUpdateTo.Contains("KEY_CONTACTS"))
                    {
                        NotifyKeyContact = true;
                    }

                    string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 50) + "\" is completed by " + username + "." + Environment.NewLine + " With comment: " + request.TaskActionReason;
                  await  _activeIncidentRepository.send_notifiation_to_groups(grp, task.ActiveIncidentId, task.ActiveIncidentTaskId, action_update, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone, NotifyKeyContact, 3, request.MessageMethod, null, request.CascadePlanID, sourceAction: SourceAction.TaskCompleted);

                    _dBCommon.DeleteScheduledJob("START_ACPT_TASK_" + task.ActiveIncidentTaskId, "TASK_SCHEDULE");
                   _dBCommon.DeleteScheduledJob("START_ESCL_TASK_" + task.ActiveIncidentTaskId, "TASK_SCHEDULE");

                    //Start the escalation job for the successor tasks
                  await _activeIncidentRepository.CreatePredecessorJobs(task.ActiveIncidentId, task.IncidentTaskId, _currentUser.UserId, _currentUser.TimeZone);

                    result.task = task;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<DeclineTaskResponse> DeclineTask(DeclineTaskRequest request)
        {
            try
            {
                MessageSourceAction = SourceAction.TaskDeclined;

                var response = new DeclineTaskResponse();
                var task = await _activeIncidentRepository.GetTaskActiveIncidentById(request.ActiveIncidentTaskID, _currentUser.CompanyId);
                if (task != null)
                {
                    var check_declined = await _activeIncidentRepository.GetTaskIncidentActionByDeline(request.ActiveIncidentTaskID);
                    foreach (var declined in check_declined)
                    {
                        if (declined.TaskActionBy == _currentUser.UserId)
                        {
                            if (task.TaskStatus != 1 && task.TaskStatus != 4 && task.TaskStatus != 6)
                            {
                                //result.ErrorId = 192;
                                response.Message = "Task already declined by you.";
                                return response;
                            }
                        }
                    }

                    bool isLastMember = false;
                    string ActionStatus = "UNALLOCATED";
                    if (task.TaskOwnerId > 0)
                    {
                        var pt_type = await _activeIncidentRepository.GetTaskActiveIncidentParticipantById(request.ActiveIncidentTaskID, _currentUser.UserId);
                        if (pt_type != null)
                        {
                            ActionStatus = pt_type.ActionStatus;
                            await _activeIncidentRepository.change_participant_type(_currentUser.UserId, request.ActiveIncidentTaskID, pt_type.PreviousParticipantTypeId, "DECLINED");
                        }
                    }
                    else
                    {
                        if (task.DelayedAccept.Year > 2000)
                        {
                            isLastMember = await _activeIncidentRepository.check_for_last_member(task.ActiveIncidentId, request.ActiveIncidentTaskID, "ESCALATION");
                        }
                        else
                        {
                            isLastMember =await  _activeIncidentRepository.check_for_last_member(task.ActiveIncidentId, request.ActiveIncidentTaskID, "ACTION");
                        }
                        if (!isLastMember)
                            await _activeIncidentRepository.change_participant_type(_currentUser.UserId, request.ActiveIncidentTaskID, 0, "DECLINED");
                    }

                    if (isLastMember)
                    {
                        //task.TaskStatus = 6;
                      
                        response.Message = "Task cannot be declined as you are the only member left.";
                        return response;
                    }
                    else
                    {
                        task.UpdatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                        task.UpdatedBy = _currentUser.UserId;
                        if (task.TaskOwnerId > 0)
                            task.TaskStatus = 3;
                    }
                   var taskActiveIncidenId= await _activeIncidentRepository.UpdateTaskActiveIncident(task);

                    var action_user = await _userRepository.GetUser(_currentUser.CompanyId, _currentUser.UserId);
                    string username = "";
                    if (action_user != null)
                    {
                        username = action_user.FirstName + " " + action_user.LastName;
                    }

                    //Add task action history
                    string task_action = "Task Declined<br/>Comment: " + request.TaskActionReason;
                   await _activeIncidentRepository.AddTaskAction(request.ActiveIncidentTaskID, task_action, _currentUser.UserId, 3, _currentUser.TimeZone);

                    bool NotifyKeyContact = false;
                    bool.TryParse(await _activeIncidentRepository.GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", _currentUser.CompanyId), out NotifyKeyContact);

                    if (ActionStatus == "REALLOCATED" && task.TaskOwnerId > 0)
                    {
                        List<NotificationUserList> TaskPtcpntList = new List<NotificationUserList>();
                        TaskPtcpntList.Add(new NotificationUserList( task.TaskOwnerId, true ));
                        string action_update = "Reallocated task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 20) + "\" declined by " + username + "." + Environment.NewLine + " Comment: " + request.TaskActionReason;

                    await   _activeIncidentRepository.notify_users(task.ActiveIncidentId, task.ActiveIncidentTaskId, TaskPtcpntList, task_action, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone, NotifyKeyContact, 3);
                    }
                    else
                    {
                        //List<string> grp = new List<string>();
                        //grp.Add("ESCALATION");
                        //string action_update = "Task " + task.TaskSequence + ": \"" + DBC.Truncate(task.TaskTitle, 20) + "\" declined with reason \"" + TaskActionReason + "\" by " + username + ".";

                        //send_notifiation_to_groups(grp, task.ActiveIncidentID, task.ActiveIncidentTaskID, action_update, CurrentUserID, CompanyID, TimeZoneId, NotifyKeyContact, 3);
                    }
                    response.result = task;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<DelegateTaskResponse> DelegateTask(DelegateTaskRequest request)
        {
            try
            {
                var Delegate = await _activeIncidentRepository.DelegateTask(request.ActiveIncidentTaskID,request.TaskActionReason, request.DelegateTo, request.MessageMethod,request.CascadePlanID,_currentUser.UserId, _currentUser.CompanyId,_currentUser.TimeZone);
                var result = _mapper.Map<TaskActiveIncident>(Delegate);
                var response = new DelegateTaskResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data has been loaded.";
                }
                else
                {
                    response.Data = result;
                    response.Message = "Cannot Deletegate!. You are no longer the owner of the task.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetActiveTaskCheckListResponse> GetActiveTaskCheckList(GetActiveTaskCheckListRequest request)
        {
            try
            {
                var activeChecks = await _activeIncidentRepository.GetActiveTaskCheckList(request.ActiveIncidentTaskID,   _currentUser.CompanyId, _currentUser.UserId);
                var result = _mapper.Map<List<ActiveCheckList>>(activeChecks);
                var response = new GetActiveTaskCheckListResponse();
                if (result != null)
                {
                    response.Data = result;
                    
                }
                else
                {
                    response.Data = result;
                   
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTaskAssignedUsersResponse> GetTaskAssignedUsers(GetTaskAssignedUsersRequest request)
        {
            try
            {
                var Delegate = await _activeIncidentRepository.GetTaskAssignedUsers(request.ActiveIncidentTaskID, request.TypeName, _currentUser.CompanyId);
                var result = _mapper.Map<List<TaskAssignedUser>>(Delegate);
                var response = new GetTaskAssignedUsersResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data has been loaded.";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No user assigned.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTaskAuditResponse> GetTaskAudit(GetTaskAuditRequest request)
        {
            try
            {
                var groups = await _activeIncidentRepository.GetTaskAudit(request.ActiveIncidentTaskID);
                var result = _mapper.Map<List<TaskAudit>>(groups);
                var response = new GetTaskAuditResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data has been loaded.";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTaskDetailsResponse> GetTaskDetails(GetTaskDetailsRequest request)
        {
            try
            {
                var details = await _activeIncidentRepository.GetTaskDetails(request.ActiveIncidentTaskID,_currentUser.CompanyId);
                
                var response = new GetTaskDetailsResponse();
                if (details != null)
                {
                    response.Data = details;
                   
                }
                else
                {
                    response.Data = details;
                   
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTaskUserListResponse> GetTaskUserList(GetTaskUserListRequest request)
        {
            try
            {
                var MainUserlist = await _activeIncidentRepository.GetTaskUserList(_paging.PageNumber,_paging.PageSize,request.Search,request.TypeName, request.ActiveIncidentTaskID, request.CompanyKey,request.OutLoginUserId,request.OutLoginCompanyId );
                var result = _mapper.Map<List<GetAllUser>>(MainUserlist);
                
                
                var response = new GetTaskUserListResponse();
                request.Search.Value = "";
               

                var TotalUsers = await _activeIncidentRepository.GetTaskUserList(_paging.PageNumber, _paging.PageSize, request.Search, request.TypeName, request.ActiveIncidentTaskID, request.CompanyKey, request.OutLoginUserId, request.OutLoginCompanyId); 

                DataTablePaging rtn = new DataTablePaging();

                if (MainUserlist != null)
                {
                    rtn.Draw = request.draw;
                    rtn.RecordsTotal = TotalUsers.Count;
                    rtn.RecordsFiltered = MainUserlist.Count;
                    rtn.Data = MainUserlist;
                    response.tablePaging = rtn;
                    response.Message = "Data Loaded.";

                }
                else
                {
                    response.tablePaging = null;
                    response.Message = "No record found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetUserTaskResponse> GetUserTasks(GetUserTaskRequest request)
        {
            try
            {
                var groups = await _activeIncidentRepository.GetUserTasks( _currentUser.UserId);
                var result = _mapper.Map<List<UserTaskHead>>(groups);
                var response = new GetUserTaskResponse();
                if (result!=null)
                {
                    response.Data = result;
                    response.Message = "Data has been loaded.";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ReallocateTaskResponse> ReallocateTask(ReallocateTaskRequest request)
        {
            try
            {
                var Delegate = await _activeIncidentRepository.ReallocateTask(request.ActiveIncidentTaskID, request.TaskActionReason, request.ReallocateTo, request.MessageMethod, request.CascadePlanID, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<TaskActiveIncident>(Delegate);
                var response = new ReallocateTaskResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Reqllocated.";
                }
                else
                {
                    response.Data = result;
                    response.Message = "Cannot Reallocate!. You are no longer the owner of the task.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SendTaskUpdateResponse> SendTaskUpdate(SendTaskUpdateRequest request)
        {
            try
            {
                bool NotifyKeyContact = false;
                var task = await _activeIncidentRepository.GetTaskActiveIncidentById(request.ActiveIncidentTaskID,_currentUser.CompanyId);
                var result = new SendTaskUpdateResponse();
                if (task != null)
                {

                    if (task.TaskOwnerId != _currentUser.UserId && task.TaskStatus != 5)
                    {
                       
                        
                        result.Message = "Cannot send update, You are no longer the owner";
                        return result;
                    }

                    List<string> grp = new List<string>();

                    //Get list of existing participant of the task
                    if (request.SendUpdateTo.Contains("ACTION_MEMBERS"))
                    {
                        grp.Add("ACTION");
                    }

                    if (request.SendUpdateTo.Contains("INCIDENT_MEMBERS"))
                    {
                        grp.Add("INCIDENT");
                    }

                    if (request.SendUpdateTo.Contains("ESCALATION_MEMBERS"))
                    {
                        grp.Add("ESCALATION");
                    }

                    //Get list of existing participant of the task
                    if (request.SendUpdateTo.Contains("KEY_CONTACTS"))
                    {
                        NotifyKeyContact = true;
                    }

                    string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 20) + "\" update. : " + request.TaskActionReason;

                  await _activeIncidentRepository.send_notifiation_to_groups(grp, task.ActiveIncidentId, task.ActiveIncidentTaskId, action_update, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone,
                        NotifyKeyContact, 3, request.MessageMethod, request.MembersToNotify, sourceAction: SourceAction.TaskUpdate);

                    await _activeIncidentRepository.AddTaskAction(request.ActiveIncidentTaskID, "Task Update: " + request.TaskActionReason, _currentUser.UserId, 8, _currentUser.TimeZone);

                    result.Data = task;
                    return result;
                }
                //Collect receipent List
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<TakeOwnershipResponse> TakeOwnership(TakeOwnershipRequest request)
        {
            try
            {
                MessageSourceAction = SourceAction.TaskTakeOver;
                var task = await _activeIncidentRepository.GetTaskActiveIncidentById(request.ActiveIncidentTaskID, _currentUser.CompanyId);
                var result = new TakeOwnershipResponse();

                if (task != null)
                {
                   
                    if (task.TaskOwnerId == _currentUser.UserId)
                    {
                        
                        result.Message = "Task already accepted";
                        return result;
                    }
                    else if (task.TaskStatus == 7)
                    {
                       
                        result.Message = "Cannot take ownership, task is completed";
                        return result;
                    }

                    int previousOwner = task.TaskOwnerId;
                    if (task.DelayedComplete.Year > 2000 && _currentUser.UserId != task.TaskOwnerId && task.TaskStatus != 7)
                    {

                        task.PreviousOwnerId = previousOwner;
                        task.TaskOwnerId = _currentUser.UserId;
                        task.UpdatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                        task.UpdatedBy = _currentUser.UserId;
                        var intUpdateId = await _activeIncidentRepository.UpdateTaskActiveIncident(task);

                        //Get Action By username
                        var action_user = await _userRepository.GetUser(_currentUser.CompanyId,_currentUser.UserId);
                        string username = "";
                        if (action_user != null)
                        {
                            username = action_user.FirstName + " " + action_user.LastName;
                        }

                        string task_action = "Task ownership is taken by " + username;

                        //Add task action history
                        await _activeIncidentRepository.AddTaskAction(request.ActiveIncidentTaskID, task_action, _currentUser.UserId, 9, _currentUser.TimeZone);

                        List<NotificationUserList> TaskPtcpntList = new List<NotificationUserList>();
                        TaskPtcpntList.Add(new NotificationUserList(previousOwner,true ));


                        bool NotifyKeyContact = false;
                        bool.TryParse(await _activeIncidentRepository.GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", _currentUser.CompanyId), out NotifyKeyContact);

                        string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 50) + "\" is now owned by " + username + ".";
                        await _activeIncidentRepository.notify_users(task.ActiveIncidentId, task.ActiveIncidentTaskId, TaskPtcpntList, action_update, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone, NotifyKeyContact, 3);

                      }
                    else if (task.DelayedComplete.Year < 2000 || new List<int> { 1, 4, 6, 7 }.Contains(task.TaskStatus))
                    {
                       
                        result.Message = "Invalid request";
                        return result;
                    }

                    result.Data = task;
                    return result;
                }
                return new TakeOwnershipResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<GetIncidentTasksAuditResponse> GetIncidentTasksAudit(GetIncidentTasksAuditRequest request)
        {
            try
            {
                var TasksAudit = await _activeIncidentRepository.GetIncidentTasksAudit(request.ActiveIncidentTaskID, request.CompanyId);
                var result = _mapper.Map<List<IncidentTaskAudit>>(TasksAudit);
                var Incident = await _activeIncidentRepository.GetIncidentActivation(request.ActiveIncidentTaskID);
                var response = new GetIncidentTasksAuditResponse();
                if (result != null && Incident!=null)
                {
                    response.Data = result;
                    response.Incident = Incident;
                }
                else
                {
                    response.Data = result;
                    response.Incident = Incident;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UnattendedTaskResponse> UnattendedTask(UnattendedTaskRequest request)
        {
            try
            {
                var TaskList = await _activeIncidentRepository.get_unattended_tasks(request.OutUserCompanyId, request.OutLoginUserId, request.ActiveIncidentID);
                
                var result = _mapper.Map<List<FailedTaskList>>(TaskList);
                var response = new UnattendedTaskResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";

                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";

                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ReassignTaskResponse> ReassignTask(ReassignTaskRequest request)
        {
            try
            {
                var TaskList = await _activeIncidentRepository.ReassignTask(request.ActiveIncidentTaskID, request.ActionUsers, request.EscalationUsers,request.TaskActionReason,request.RemoveCurrentOwner, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);

                var result = _mapper.Map<TaskActiveIncident>(TaskList);
                var response = new ReassignTaskResponse();
                if (result != null)
                {
                    response.data = result;
                    response.Message = "Data Loaded";

                }
                else
                {
                    response.data = result;
                    response.Message = "No record found.";

                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<NewAdHocTaskResponse> NewAdHocTask(NewAdHocTaskRequest request)
        {
            try
            {
                var TaskList = await _activeIncidentRepository.NewAdHocTask(request.ActiveIncidentID,request.TaskTitle,request.TaskDescription, request.ActionUsers,request.ActionGroups,request.EscalationUsers,request.EscalationGroups,request.EscalationDuration,request.ExpectedCompletionTime, _currentUser.CompanyId, _currentUser.UserId, _currentUser.TimeZone);

                var result = _mapper.Map<int>(TaskList);
                var response = new NewAdHocTaskResponse();
                if (result >0)
                {
                    response.result = result;
                   
                }
                else
                {
                    response.result = result;
                    
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AddNotesResponse> AddNotes(AddNotesRequest request)
        {
            try
            {
                var TaskAction = await _activeIncidentRepository.AddTaskAction(request.ActiveIncidentTaskID, request.TaskCompletionNote, _currentUser.UserId, 10, _currentUser.TimeZone);

                var result = _mapper.Map<int>(TaskAction);
                var response = new AddNotesResponse();
                if (result > 0)
                {
                    response.result = result;

                }
                else
                {
                    response.result = result;

                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveActiveCheckListResponse> SaveActiveCheckListResponse(SaveActiveCheckListResponseRequest request)
        {
            try
            {
             var task= await _activeIncidentRepository.SaveActiveCheckListResponse(request.ActiveIncidentTaskID, request.CheckListResponse, _currentUser.UserId, _currentUser.CompanyId,_currentUser.TimeZone);

                var result = _mapper.Map<bool>(task);
                var response = new SaveActiveCheckListResponse();
                if (result)
                {
                    response.result = true;
                    response.Message = "Addedd";

                }
                else
                {
                    response.result = false;
                    response.Message = "Not added";

                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
