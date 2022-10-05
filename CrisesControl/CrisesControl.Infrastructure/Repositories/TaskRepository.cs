using CrisesControl.Core.Common;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.Repositories;
using CrisesControl.Core.Tasks.SP_Response;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly CrisesControlContext _context;
    public TaskRepository(CrisesControlContext context)
    {
        _context = context;
    }

    public TaskHeaderWithName GeTaskHeader(int incidentId)
    {
        var taskHeader = (from TH in _context.Set<TaskHeader>().AsEnumerable()
                          where TH.IncidentId == incidentId
                          select new TaskHeaderWithName
                          {
                              Author = TH.Author,
                              IncidentId = TH.IncidentId,
                              IsActive = TH.IsActive,
                              NextReviewDate = TH.NextReviewDate,
                              ReminderCount = TH.ReminderCount,
                              ReviewFrequency = TH.ReviewFrequency,
                              Rpo = TH.Rpo,
                              Rto = TH.Rto,
                              SendReminder = TH.SendReminder,
                              TaskHeaderId = TH.TaskHeaderId,
                              UpdatedBy = TH.UpdatedBy,
                              UpdatedOn = TH.UpdatedOn,
                              TaskAuthorName = (from U in _context.Set<User>().AsEnumerable()
                                                where U.UserId == TH.Author
                                                select new UserFullName { Firstname = U.FirstName, Lastname = U.LastName }).FirstOrDefault()
                          }).FirstOrDefault();
        return taskHeader;
    }

    public TaskHeader? GeTaskHeader(int incidentId, int taskHeaderId)
    {
        return (from TH in _context.Set<TaskHeader>().AsEnumerable()
                where TH.IncidentId == incidentId && TH.TaskHeaderId == taskHeaderId
                select TH).FirstOrDefault();
    }

    public async Task<List<TaskDetails>> GetTasks(int incidentId, int incidentTaskId, bool single = false, int companyId = 0, int? taskHeaderId = null, CancellationToken cancellationToken = default)
    {
        int taskHeaderIdInt;
        if (taskHeaderId == null)
        {
            var taskHeader = GeTaskHeader(incidentId);
            if (taskHeader == null)
            {
                return new List<TaskDetails>() { };
            }
            taskHeaderIdInt = Convert.ToInt32(taskHeader.TaskHeaderId);
        }
        else
        {
            taskHeaderIdInt = Convert.ToInt32(taskHeaderId);
        }

        var ptType = (from PT in _context.Set<TaskParticipantType>().AsEnumerable() select PT).ToList();
        int actionType = ptType.Where(w => w.TaskParticipantTypeName == "ACTION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();
        int escalType = ptType.Where(w => w.TaskParticipantTypeName == "ESCALATION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();

        if (!single)
        {
            //TODO: Possible write operation, should probably remove the below line because this function is invoked from a query.
            await FixTaskOrder(incidentId, taskHeaderIdInt, cancellationToken);

            return (from TL in _context.Set<TaskIncident>().AsEnumerable()
                    where TL.IncidentId == incidentId && TL.TaskHeaderId == taskHeaderIdInt && TL.CompanyId == companyId
                    select new TaskDetails
                    {
                        TaskTitle = TL.TaskTitle,
                        TaskDescription = TL.TaskDescription,
                        TaskHeaderId = TL.TaskHeaderId,
                        TaskSequence = TL.TaskSequence,
                        EscalationDuration = TL.EscalationDuration,
                        ExpectedCompletionTime = TL.ExpectedCompletionTime,
                        HasPredecessor = TL.HasPredecessor,
                        IncidentId = TL.IncidentId,
                        IncidentTaskId = TL.IncidentTaskId,
                        Status = TL.Status,
                    }).OrderBy(o => o.TaskSequence).ToList()
                                .Select(s =>
                                {
                                    s.TaskPredecessor = GetTaskPredecessor(s.IncidentTaskId, s.IncidentId);
                                    s.ActionGroups = GetTaskGroup(s.IncidentTaskId, s.TaskHeaderId, actionType);
                                    s.EscalationGroups = GetTaskGroup(s.IncidentTaskId, s.TaskHeaderId, escalType);
                                    s.ActionUsers = GetTaskUsers(s.IncidentTaskId, s.TaskHeaderId, actionType);
                                    s.EscalationUsers = GetTaskUsers(s.IncidentTaskId, s.TaskHeaderId, escalType);
                                    return s;
                                }).ToList();
        }
        else
        {
            return (from TL in _context.Set<TaskIncident>().AsEnumerable()
                    where TL.IncidentId == incidentId && TL.TaskHeaderId == taskHeaderIdInt && TL.IncidentTaskId == incidentTaskId
                    && TL.CompanyId == companyId
                    select new TaskDetails
                    {
                        TaskTitle = TL.TaskTitle,
                        TaskDescription = TL.TaskDescription,
                        TaskHeaderId = TL.TaskHeaderId,
                        TaskSequence = TL.TaskSequence,
                        EscalationDuration = TL.EscalationDuration,
                        ExpectedCompletionTime = TL.ExpectedCompletionTime,
                        HasPredecessor = TL.HasPredecessor,
                        IncidentId = TL.IncidentId,
                        IncidentTaskId = TL.IncidentTaskId,
                        Status = TL.Status,
                    }).OrderBy(o => o.TaskSequence).ToList()
                                .Select(s =>
                                {
                                    s.TaskPredecessor = GetTaskPredecessor(s.IncidentTaskId, s.IncidentId);
                                    s.ActionGroups = GetTaskGroup(s.IncidentTaskId, s.TaskHeaderId, actionType);
                                    s.EscalationGroups = GetTaskGroup(s.IncidentTaskId, s.TaskHeaderId, escalType);
                                    s.ActionUsers = GetTaskUsers(s.IncidentTaskId, s.TaskHeaderId, actionType);
                                    s.EscalationUsers = GetTaskUsers(s.IncidentTaskId, s.TaskHeaderId, escalType);
                                    return s;
                                }).ToList();
        }
    }

    public List<CheckListOption> GetChkResponseOptions(int companyId, int userId)
    {
        var pCompanyID = new SqlParameter("@CompanyID", companyId);
        var pUserID = new SqlParameter("@UserID", userId);
        var qResult = _context.Set<GetCheckListReponseOption>().FromSqlRaw("Pro_Get_CheckList_Reponse_Option @CompanyID, @UserID",
            pCompanyID, pUserID).ToList();
        List<CheckListOption> result = new();
        foreach (var option in qResult ?? Enumerable.Empty<GetCheckListReponseOption>())
        {
            result.Add(new CheckListOption() { ResponseId = option.ResponseId, Response = option.Response, MarkDone = option.MarkDone });
        }
        return result;
    }

    public List<CheckListUpsert> GetTaskCheckList(int incidentTaskId, int companyId, int userId)
    {
        var pIncidentTaskID = new SqlParameter("@IncidentTaskID", incidentTaskId);
        var pCompanyID = new SqlParameter("@CompanyID", companyId);
        var pUserID = new SqlParameter("@UserID", userId);
        var qResult = _context.Set<JsonResult>().FromSqlRaw("Pro_Get_TaskCheckList @IncidentTaskID, @CompanyID, @UserID", pIncidentTaskID, pCompanyID, pUserID).ToList()?.FirstOrDefault();
        return qResult?.Result != null ?
            JsonConvert.DeserializeObject<List<CheckListUpsert>>(qResult.Result)
            : new List<CheckListUpsert>();
    }

    public async Task<int> CreateTaskHeader(int taskHeaderId, int incidentId, int author, DateTimeOffset nextReviewDate, string reviewFrequency,
                        bool sendReminder, bool isActive, decimal rto, decimal rpo, int currentUserId, int companyId, string timeZoneId, CancellationToken cancellationToken)
    {
        int reminderCount = 0;
        if (taskHeaderId <= 0)
        {
            TaskHeader TH = new TaskHeader();
            TH.Author = author;
            TH.IncidentId = incidentId;
            TH.NextReviewDate = nextReviewDate;
            TH.ReviewFrequency = reviewFrequency;
            TH.SendReminder = sendReminder;
            TH.CreatedBy = currentUserId;
            TH.CreatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
            TH.UpdatedBy = currentUserId;
            TH.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
            TH.ReminderCount = reminderCount;
            TH.IsActive = isActive;
            TH.Rto = rto;
            TH.Rpo = rpo;
            await _context.AddAsync(TH);
            await _context.SaveChangesAsync(cancellationToken);
            //TODO: Save in audit log
            //db.SaveChanges(currentUserId, companyId);
            taskHeaderId = TH.TaskHeaderId;
        }
        else
        {
            var taskHeader = (from TH in _context.Set<TaskHeader>().AsEnumerable()
                               join I in _context.Set<Incident>().AsEnumerable() on TH.IncidentId equals I.IncidentId
                               where I.CompanyId == companyId && TH.TaskHeaderId == taskHeaderId
                               select TH).FirstOrDefault();
            if (taskHeader != null)
            {
                taskHeader.Author = author;
                taskHeader.NextReviewDate = nextReviewDate;
                taskHeader.ReviewFrequency = reviewFrequency;
                taskHeader.ReminderCount = 0;
                taskHeader.SendReminder = sendReminder;
                taskHeader.IsActive = isActive;
                taskHeader.UpdatedBy = currentUserId;
                taskHeader.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                taskHeader.Rto = rto;
                taskHeader.Rpo = rpo;
                await _context.SaveChangesAsync(cancellationToken);
                //TODO: Save in audit log
                //db.SaveChanges(currentUserId, companyId);
                reminderCount = taskHeader.ReminderCount;
            }
        }

        //TODO: Delete all relevant scheduled jobs
        //if (!isActive || sendReminder == false)
        //{
        //    DBC.DeleteScheduledJob("TASKS_REVIEW_" + incidentId, "REVIEW_REMINDER");
        //}
        //else if (isActive == true && sendReminder == true)
        //{
        //    CreateTasksReviewReminder(incidentId, taskHeaderId, companyId, nextReviewDate, reviewFrequency, reminderCount);
        //}

        await FixTaskOrder(incidentId, taskHeaderId, cancellationToken);
        return taskHeaderId;
    }

    public async Task<int> CreateTask(int incidentTaskId, int taskHeaderId, int incidentId, string taskTitle, string taskDescription, int taskStatus, double escalationDuration, double expectedCompletionTime,
                        bool hasPredecessor, int updateUserId, int companyId, string timeZoneId, CancellationToken cancellationToken)
    {
        int currentSequence = (from T in _context.Set<TaskIncident>().AsEnumerable()
                                where T.TaskHeaderId == taskHeaderId && T.Status == 1
                                select T).Count();
        if (incidentTaskId <= 0)
        {
            TaskIncident T = new TaskIncident();
            T.CompanyId = companyId;
            T.IncidentId = incidentId;
            T.TaskHeaderId = taskHeaderId;
            T.TaskTitle = taskTitle;
            T.TaskDescription = taskDescription;
            T.HasPredecessor = hasPredecessor;
            T.TaskSequence = currentSequence + 1;
            T.EscalationDuration = escalationDuration;
            T.ExpectedCompletionTime = expectedCompletionTime;
            T.Status = taskStatus;
            T.CreatedBy = updateUserId;
            T.CreatedDate = DateTime.Now.GetDateTimeOffset(timeZoneId);
            T.UpdatedBy = updateUserId;
            T.UpdatedDate = DateTime.Now.GetDateTimeOffset(timeZoneId);

            await _context.AddAsync(T);
            await _context.SaveChangesAsync(cancellationToken);
            //TODO: Save in audit log
            //db.SaveChanges(updateUserId, companyId);
            incidentTaskId = T.IncidentTaskId;
        }
        else
        {
            var task = (from T in _context.Set<TaskIncident>().AsEnumerable()
                        where T.IncidentTaskId == incidentTaskId
                        select T).FirstOrDefault();
            if (task != null)
            {
                task.TaskTitle = taskTitle;
                task.TaskDescription = taskDescription;
                task.HasPredecessor = hasPredecessor;
                task.EscalationDuration = escalationDuration;
                task.ExpectedCompletionTime = expectedCompletionTime;
                task.Status = taskStatus;
                task.UpdatedBy = updateUserId;
                task.UpdatedDate = DateTime.Now.GetDateTimeOffset(timeZoneId);
                await _context.SaveChangesAsync(cancellationToken);
                //TODO: Save in audit log
                //db.SaveChanges(updateUserId, companyId);
            }
        }
        await FixTaskOrder(incidentId, taskHeaderId, cancellationToken);
        return incidentTaskId;
    }


    public async Task SaveTaskCheckLists(List<CheckList> checkLists, int incidentTaskId, int updateUserId, int companyId, string timeZoneId, CancellationToken cancellationToken)
    {
        List<int> ChkAddedTracker = new List<int>();
        foreach (CheckList checkList in checkLists ?? Enumerable.Empty<CheckList>())
        {
            int checkListId;
            //Add/update TaskCheckList table
            if (checkList.CheckListId <= 0)
            {
                TaskCheckList TCL = new TaskCheckList
                {
                    TaskId = incidentTaskId,
                    Description = checkList.Description,
                    OptionCount = checkList.OptionCount,
                    SortOrder = checkList.SortOrder,
                    DoneOnly = checkList.DoneOnly,
                    CreatedBy = updateUserId,
                    CreatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId),
                    UpdatedBy = updateUserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId)
                };
                await _context.AddAsync(TCL);
                await _context.SaveChangesAsync(cancellationToken);
                //TODO: Save in audit log
                //db.SaveChanges(updateUserId, companyId);
                checkListId = TCL.CheckListId;
            }
            else
            {
                var taskCheckList = (from TCL in _context.Set<TaskCheckList>().AsEnumerable()
                                     where TCL.CheckListId == checkList.CheckListId
                                     select TCL).FirstOrDefault();
                if (taskCheckList != null)
                {
                    taskCheckList.TaskId = incidentTaskId;
                    taskCheckList.Description = checkList.Description;
                    taskCheckList.DoneOnly = checkList.DoneOnly;
                    taskCheckList.OptionCount = checkList.OptionCount;
                    taskCheckList.SortOrder = checkList.SortOrder;
                    taskCheckList.UpdatedBy = updateUserId;
                    taskCheckList.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                }
                checkListId = taskCheckList.CheckListId;
                await _context.SaveChangesAsync(cancellationToken);
                //TODO: Save in audit log
                //db.SaveChanges(updateUserId, companyId);
            }

            ChkAddedTracker.Add(checkListId);

            List<int> AddedTracker = new List<int>();

            var opts = (from OP in _context.Set<TaskCheckListResponse>().AsEnumerable() where OP.CheckListId == checkListId select OP).ToList();
            foreach (CheckListOption checkListOption in checkList.CheckListOptions ?? Enumerable.Empty<CheckListOption>())
            {
                //Add/update TaskCheckListResponse table
                var isexist = (from op in opts where op.CheckListId == checkListId && op.ResponseId == checkListOption.ResponseId select op).Any();

                if (!isexist)
                {
                    TaskCheckListResponse TCLR = new TaskCheckListResponse
                    {
                        CheckListId = checkListId,
                        ResponseId = checkListOption.ResponseId,
                        CreatedBy = updateUserId,
                        CreatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId),
                        UpdatedBy = updateUserId,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId)
                    };
                    await _context.AddAsync(TCLR);
                    await _context.SaveChangesAsync(cancellationToken);
                    //TODO: Save in audit log
                    //db.SaveChanges(updateUserId, companyId);
                    AddedTracker.Add(TCLR.ResponseId);
                }
                else
                {
                    var taskCheckListResponse = (from TCLR in opts
                                                 where TCLR.ResponseId == checkListOption.ResponseId
                                                 select TCLR).FirstOrDefault();
                    if (taskCheckListResponse != null)
                    {
                        taskCheckListResponse.CheckListId = checkListId;
                        taskCheckListResponse.ResponseId = checkListOption.ResponseId;
                        taskCheckListResponse.UpdatedBy = updateUserId;
                        taskCheckListResponse.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    //TODO: Save in audit log
                    //db.SaveChanges(updateUserId, companyId);
                    AddedTracker.Add(taskCheckListResponse.ResponseId);
                }
            }

            //Delete TaskCheckListResponse
            var checkListResponseToDelete = (from TCLR in _context.Set<TaskCheckListResponse>().AsEnumerable()
                                             where (TCLR.CheckListId == checkListId && !AddedTracker.Contains(TCLR.ResponseId))
                                             select TCLR).ToList();
            _context.RemoveRange(checkListResponseToDelete);
            await _context.SaveChangesAsync(cancellationToken);
            //TODO: Save in audit log
            //db.SaveChanges(updateUserId, companyId);
        }

        //Delete CheckList
        var checkListToDelete = _context.Set<TaskCheckList>().AsEnumerable().Where(x => x.TaskId == incidentTaskId && !ChkAddedTracker.Contains(x.CheckListId)).ToList();
        _context.RemoveRange(checkListToDelete);
        await _context.SaveChangesAsync(cancellationToken);
        //TODO: Save in audit log
        //db.SaveChanges(updateUserId, companyId);
    }

    public async Task IncidentTaskParticipants(int incidentTaskId, int[] actionUsers, int[] actionGroups, int[] escalationUsers, int[] escalationGroups, CancellationToken cancellationToken)
    {
        var ptType = (from PT in _context.Set<TaskParticipantType>().AsEnumerable() select PT).ToList();
        int actionType = ptType.Where(w => w.TaskParticipantTypeName == "ACTION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();
        int escalType = ptType.Where(w => w.TaskParticipantTypeName == "ESCALATION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();

        var pt_list = (from IPD in _context.Set<TaskIncidentParticipant>().AsEnumerable() where IPD.IncidentTaskId == incidentTaskId select IPD).ToList();
        _context.RemoveRange(pt_list);
        await _context.SaveChangesAsync(cancellationToken);

        //Create action list.
        if (actionUsers != null)
            await CreateParticipantList(actionUsers, incidentTaskId, actionType, "USER", cancellationToken);
        if (actionGroups != null)
            await CreateParticipantList(actionGroups, incidentTaskId, actionType, "GROUP", cancellationToken);

        //Create escalation list
        if (escalationUsers != null)
            await CreateParticipantList(escalationUsers, incidentTaskId, escalType, "USER", cancellationToken);
        if (escalationGroups != null)
            await CreateParticipantList(escalationGroups, incidentTaskId, escalType, "GROUP", cancellationToken);
    }

    public async Task IncidentTaskPredecessors(int incidentTaskId, int[] predecessors, CancellationToken cancellationToken)
    {
        var oldPdList = (from IPD in _context.Set<TaskIncidentPredecessor>().AsEnumerable() where IPD.IncidentTaskId == incidentTaskId select IPD).ToList();
        if (oldPdList != null)
        {
            _context.RemoveRange(oldPdList);
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (predecessors.Length > 0)
        {
            foreach (int pd in predecessors)
            {
                TaskIncidentPredecessor ITP = new TaskIncidentPredecessor()
                {
                    IncidentTaskId = incidentTaskId,
                    PredecessorTaskId = pd
                };
                _context.Add(ITP);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ReorderTask(int incidentId, int taskHeaderId, List<TaskSequence> taskSequences, CancellationToken cancellationToken)
    {
        var incidentTasks = (from IT in _context.Set<TaskIncident>().AsEnumerable() where IT.IncidentId == incidentId && IT.TaskHeaderId == taskHeaderId select IT).ToList();
        foreach (TaskSequence ts in taskSequences)
        {
            var getTask = (from I in incidentTasks where I.IncidentTaskId == ts.IncidentTaskId select I).FirstOrDefault();
            if (getTask != null)
            {
                getTask.TaskSequence = ts.NewPosition;
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
        await FixTaskOrder(incidentId, taskHeaderId, cancellationToken);
    }

    public List<CrisesControl.Core.Assets.Assets> GetTaskAsset(int incidentTaskId, int companyId)
    {
        var pIncidentTaskID = new SqlParameter("@IncidentTaskID", incidentTaskId);
        var pCompanyID = new SqlParameter("@CompanyID", companyId);
        return _context.Set<CrisesControl.Core.Assets.Assets>().FromSqlRaw("Pro_Get_Task_Assets @IncidentTaskID, @CompanyID", pIncidentTaskID, pCompanyID).ToList();
    }

    public async Task SaveTaskAssets(int incidentTaskId, int[] taskAssets, int currentUserId, int companyId, string timeZoneId, CancellationToken cancellationToken)
    {
        int ObjMapId = (from O in _context.Set<Core.Models.Object>().AsEnumerable()
                        join OM in _context.Set<ObjectMapping>().AsEnumerable() on O.ObjectId equals OM.TargetObjectId
                        where O.ObjectName == "IncidentTask"
                        select OM.ObjectMappingId).FirstOrDefault();

        foreach (int AssetId in taskAssets)
        {

            ObjectRelation tblObjRel = new ObjectRelation()
            {
                TargetObjectPrimaryId = incidentTaskId,
                ObjectMappingId = ObjMapId,
                SourceObjectPrimaryId = AssetId,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId,
                CreatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId),
                UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId)
            };
            _context.Add(tblObjRel);
        }
        await _context.SaveChangesAsync(cancellationToken);
        //TODO: Save in audit log
        //db.SaveChanges(CurrentUserId, CompanyId);
    }

    public async Task DeleteTaskAsset(int incidentTaskId, int[] taskAssets, int currentUserId, int companyId, CancellationToken cancellationToken)
    {
        int delObjMapId = (from O in _context.Set<Core.Models.Object>().AsEnumerable()
                           join OM in _context.Set<ObjectMapping>().AsEnumerable() on O.ObjectId equals OM.TargetObjectId
                           where O.ObjectName == "IncidentTask"
                           select OM.ObjectMappingId).FirstOrDefault();

        var QueryRec = (from OR in _context.Set<ObjectRelation>()
                        where OR.TargetObjectPrimaryId == incidentTaskId
                        && OR.ObjectMappingId == delObjMapId
                        && taskAssets.Contains(OR.SourceObjectPrimaryId)
                        select OR).ToList();
        _context.RemoveRange(QueryRec);
        await _context.SaveChangesAsync(cancellationToken);
        //TODO: Save in audit log
        //db.SaveChanges(CurrentUserId, CompanyId);
    }

    public async Task DeleteTask(int incidentTaskId, int taskHeaderId, CancellationToken cancellationToken)
    {
        var task = (from T in _context.Set<TaskIncident>().AsEnumerable()
                    where T.IncidentTaskId == incidentTaskId
                    select T).FirstOrDefault();
        if (task != null)
        {
            int sequence = task.TaskSequence;
            var emptylist = new List<int>();
            int[] emptyarray = emptylist.ToArray();
            await IncidentTaskPredecessors(incidentTaskId, emptyarray, cancellationToken);

            var getIncidentSuccess = (from SC in _context.Set<TaskIncidentPredecessor>().AsEnumerable() where SC.PredecessorTaskId == incidentTaskId select SC).Distinct().ToList();
            if (getIncidentSuccess != null)
            {
                _context.RemoveRange(getIncidentSuccess);
            }

            var recepients = (from TR in _context.Set<TaskIncidentParticipant>().AsEnumerable() where TR.IncidentTaskId == incidentTaskId select TR).ToList();
            _context.RemoveRange(recepients);

            _context.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);

            var ordertasks = (from OT in _context.Set<TaskIncident>().AsEnumerable() where OT.TaskSequence > sequence select OT).ToList();
            foreach (var ordtask in ordertasks)
            {
                ordtask.TaskSequence = ordtask.TaskSequence - 1;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task CloneTask(int incidentTaskId, int incidentId, int companyId, int userId, string timeZoneId, CancellationToken cancellationToken)
    {
        DateTimeOffset dtNow = DateTime.Now.GetDateTimeOffset(timeZoneId);
        var pIncidentTaskID = new SqlParameter("@IncidentTaskID", incidentTaskId);
        var pIncidentID = new SqlParameter("@IncidentID", incidentId);
        var pCompanyID = new SqlParameter("@CompanyID", companyId);
        var pUserID = new SqlParameter("@CurrentUserID", userId);
        var pCustomerTime = new SqlParameter("@CustomerTime", dtNow);
        await _context.Database.ExecuteSqlRawAsync("Clone_Incident_Task @IncidentID, @IncidentTaskID, @CurrentUserID, @CompanyID, @CustomerTime",
            new object[] { pIncidentTaskID, pIncidentID, pUserID, pCompanyID, pCustomerTime },
            cancellationToken);
    }
    
    
    private async Task FixTaskOrder(int incidentId, int taskHeaderId, CancellationToken cancellationToken)
    {
        var rearrangeTasks = await _context.Set<TaskIncident>()
                               .Where(IT=> IT.IncidentId == incidentId &&
                                IT.TaskHeaderId == taskHeaderId)
                               .OrderBy(IT=> IT.TaskSequence).ToListAsync();
        int newseq = 1;
        foreach (var rtask in rearrangeTasks)
        {
            if (rtask.TaskSequence != newseq)
                rtask.TaskSequence = newseq;
            newseq++;
        }
        await _context.SaveChangesAsync(cancellationToken);

        var inci = await _context.Set<Incident>().Where(I=> I.IncidentId == incidentId).FirstOrDefaultAsync();
        if (inci != null)
        {
            if (rearrangeTasks.Count <= 0)
            {
                inci.HasTask = false;
            }
            else
            {
                inci.HasTask = true;
            }
            _context.Update(inci);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private List<Predecessor> GetTaskPredecessor(int incidentTaskId, int incidentId)
    {
        var pIncidentTaskID = new SqlParameter("@IncidentTaskID", incidentTaskId);
        var pIncidentID = new SqlParameter("@IncidentID", incidentId);
        return _context.Set<Predecessor>().FromSqlRaw("Pro_Task_Predecessor @IncidentTaskID, @IncidentID", pIncidentTaskID, pIncidentID).ToList();
    }

    private List<TaskGroup> GetTaskGroup(int incidentTaskId, int taskHeaderId, int groupType)
    {
        var pIncidentTaskID = new SqlParameter("@IncidentTaskID", incidentTaskId);
        var pTaskHeaderID = new SqlParameter("@TaskHeaderID", taskHeaderId);
        var pGroupType = new SqlParameter("@GroupType", groupType);
        return _context.Set<TaskGroup>().FromSqlRaw("Pro_Task_Groups @IncidentTaskID, @TaskHeaderID, @GroupType",
            pIncidentTaskID, pTaskHeaderID, pGroupType).ToList();
    }

    private List<TaskUser> GetTaskUsers(int incidentTaskId, int taskHeaderId, int groupType)
    {
        var pIncidentTaskID = new SqlParameter("@IncidentTaskID", incidentTaskId);
        var pTaskHeaderID = new SqlParameter("@TaskHeaderID", taskHeaderId);
        var pGroupType = new SqlParameter("@GroupType", groupType);
        return _context.Set<TaskUser>().FromSqlRaw("Pro_Task_Users @IncidentTaskID, @TaskHeaderID, @GroupType",
            pIncidentTaskID, pTaskHeaderID, pGroupType).ToList();
    }

    private async Task CreateParticipantList(int[] uList, int incidentTaskId, int paticipentTypeId, string objtype, CancellationToken cancellationToken)
    {
        if (uList != null)
        {
            foreach (int item in uList)
            {
                if (incidentTaskId > 0 && paticipentTypeId > 0)
                {
                    TaskIncidentParticipant ITP = new TaskIncidentParticipant();
                    ITP.IncidentTaskId = incidentTaskId;
                    ITP.ParticipantTypeId = paticipentTypeId;
                    ITP.ParticipantUserId = (objtype == "USER" ? item : 0);
                    ITP.ParticipantGroupId = (objtype == "GROUP" ? item : 0);
                    _context.Add(ITP);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}