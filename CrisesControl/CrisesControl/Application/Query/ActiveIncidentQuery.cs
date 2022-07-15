using AutoMapper;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
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
        private readonly ICompanyRepository _companyRepository;
        private readonly DBCommon _dBCommon;
        private string MessageSourceAction = string.Empty;
        public ActiveIncidentQuery(IActiveIncidentRepository activeIncidentRepository, IUserRepository userRepository, IMapper mapper, ILogger<ActiveIncidentQuery> logger, ICurrentUser currentUser, ICompanyRepository companyRepository, DBCommon dBCommon)
        {
            this._activeIncidentRepository = activeIncidentRepository;
            this._mapper = mapper;
            this._logger = logger;
            this._currentUser = currentUser;
            this._userRepository = userRepository;
            this._companyRepository = companyRepository;
            this._dBCommon = dBCommon;
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
                    bool.TryParse(await _companyRepository.GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", _currentUser.CompanyId), out NotifyKeyContact);

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
                    bool.TryParse(await _companyRepository.GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", _currentUser.CompanyId), out NotifyKeyContact);

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
    }
}
