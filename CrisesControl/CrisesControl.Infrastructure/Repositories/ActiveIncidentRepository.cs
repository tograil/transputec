using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Common;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CrisesControl.Core.Reports;
using Location = CrisesControl.Core.Locations.Location;
using Object = CrisesControl.Core.Models.Object;

namespace CrisesControl.Infrastructure.Repositories;

public class ActiveIncidentRepository : IActiveIncidentRepository
{
    private readonly CrisesControlContext _context;
    public int UserTaskCount = 0;
    public bool IsFundAvailable = true;
    private string MessageSourceAction = string.Empty;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private IConfiguration _configuration;

    public ActiveIncidentRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor,  IConfiguration configuration)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task ProcessKeyHolders(int companyId, int incidentId, int activeIncidentId, int currentUserId,
        int[] incidentKeyHolders)
    {
        var deleteExt = await _context.Set<IncidentKeyholder>().Where(x => x.IncidentID == incidentId
                                                                     && x.ActiveIncidentID == activeIncidentId)
            .ToListAsync();

        _context.RemoveRange(deleteExt);

        await _context.SaveChangesAsync();

        var keyHoldersToSave = incidentKeyHolders.Select(x => new IncidentKeyholder
        {
            CompanyID = companyId,
            IncidentID = incidentId,
            ActiveIncidentID = activeIncidentId,
            UserID = x
        }).ToList();

        _context.AddRange(keyHoldersToSave);
        await _context.SaveChangesAsync();
    }

    public async Task ProcessImpactedLocation(int[] locationIds, int incidentActivationId, int companyId, string action)
    {
        var locations = await _context.Set<Location>()
            .Where(x => locationIds.Contains(x.LocationId))
            .Select(x => new AffectedLocation
            {
                Address = x.PostCode,
                Lat = x.Lat,
                Lng = x.Long,
                LocationID = 0,
                LocationName = x.LocationName,
                LocationType = "IMPACTED",
                ImpactedLocationID = x.LocationId
            }).ToListAsync();

        await ProcessAffectedLocation(locations, incidentActivationId, companyId, "IMPACTED", action);
    }

    public async Task ProcessAffectedLocation(ICollection<AffectedLocation> affectedLocations,
        int incidentActivationId,
        int companyId, string type = "AFFECTED", string action = "INITIATE")
    {
        if (action == "LAUNCH")
        {
            var extLoc = await _context.Set<IncidentLocation>().Include(x => x.IncidentLocationLibrary)
                .Where(x => x.IncidentActivationId == incidentActivationId
                && x.IncidentLocationLibrary.LocationType == type
                && !affectedLocations.Select(s => s.LocationID).Contains(x.LocationId))
                .ToListAsync();

            _context.Set<IncidentLocation>().RemoveRange(extLoc);
            await _context.SaveChangesAsync();
        }

        foreach (var affectedLocation in affectedLocations)
        {
            await CreateIncidentLocation(affectedLocation, incidentActivationId, companyId);
        }
    }

    public async Task<ICollection<IncidentActivation>> GetIncidentActivationList(int incidentActivationId, int companyId)
    {
        return await _context.Set<IncidentActivation>()
            .Where(x => x.CompanyId == companyId && x.IncidentActivationId == incidentActivationId)
            .ToListAsync();
    }

    public async Task CreateActiveKeyContact(int incidentActivationId, int incidentId, IncidentKeyHldLst[] keyHldLst, int currentUserId,
        int companyId, string timeZoneId)
    {
        var activeIncidentKeyContacts = await _context.Set<ActiveIncidentKeyContact>()
            .Where(x => x.IncidentActivationId == incidentActivationId)
            .ToListAsync();

        var aiKList = new List<int>();
        foreach (var incidentKeyHldLst in keyHldLst)
        {
            if (incidentKeyHldLst.UserId is not null)
            {
                var activeIncidentKey = activeIncidentKeyContacts
                    .FirstOrDefault(s => s.IncidentActivationId == incidentActivationId
                                         && s.IncidentId == incidentId && s.UserId == incidentKeyHldLst.UserId);
                if (activeIncidentKey is null)
                {
                    var incKeyContact = new ActiveIncidentKeyContact
                    {
                        IncidentActivationId = incidentActivationId,
                        IncidentId = incidentId,
                        UserId = incidentKeyHldLst.UserId.Value,
                        CreatedBy = currentUserId,
                        CreatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId),
                        UpdatedBy = currentUserId,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId)
                    };
                    await _context.AddAsync(incKeyContact);
                }
                else
                {
                    aiKList.Add(activeIncidentKey.ActiveIncidentKeyContactId);
                }
            }
        }

        foreach (var activeIncidentKeyContact in activeIncidentKeyContacts)
        {
            var isDel = aiKList.Any(s => s == activeIncidentKeyContact.ActiveIncidentKeyContactId);
            if (!isDel)
            {
                _context.Remove(activeIncidentKeyContact);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> CreateActiveIncidentTask(int activeIncidentTaskId, int activeIncidentId, int incidentTaskId, string taskTitle,
        string taskDescription, bool hasPredecessor, double escalationDuration, double expectedCompletionTime,
        int taskSequence, int taskOwnerId, DateTime taskAcceptedDate, DateTime taskEscalatedDate, int taskStatus,
        int taskCompletedBy, int nextIncidentTaskId, int previousIncidentTaskId, int previousOwnerId,
        DateTimeOffset taskActivationDate, int currentUserId, int companyId)
    {
        if (activeIncidentTaskId <= 0)
        {
            var ait = new TaskActiveIncident
            {
                IncidentTaskId = incidentTaskId,
                ActiveIncidentId = activeIncidentId,
                CompanyId = companyId,
                TaskOwnerId = taskOwnerId,
                TaskEscalatedDate = taskEscalatedDate,
                TaskStatus = taskStatus,
                TaskCompletedBy = taskCompletedBy,
                NextIncidentTaskId = nextIncidentTaskId,
                PreviousIncidentTaskId = previousIncidentTaskId,
                TaskActivationDate = taskActivationDate,
                PreviousOwnerId = previousOwnerId,
                TaskSequence = taskSequence,
                TaskTitle = taskTitle,
                TaskDescription = taskDescription,
                HasPredecessor = hasPredecessor,
                EscalationDuration = escalationDuration,
                ExpectedCompletionTime = expectedCompletionTime,
                UpdatedDate = DateTime.Now.GetDateTimeOffset(),
                UpdatedBy = currentUserId,
                DelayedAccept = (DateTime)SqlDateTime.Null,
                DelayedComplete = (DateTime)SqlDateTime.Null,
                HasCheckList = 0
            };

            await _context.AddAsync(ait);
            await _context.SaveChangesAsync();

            return ait.ActiveIncidentTaskId;
        }
        else
        {
            var getTask = await _context.Set<TaskActiveIncident>()
                .FirstOrDefaultAsync(x => x.ActiveIncidentTaskId == activeIncidentTaskId);

            if (getTask is not null)
            {
                getTask.NextIncidentTaskId = nextIncidentTaskId;
                getTask.PreviousIncidentTaskId = previousIncidentTaskId;
                getTask.PreviousOwnerId = previousOwnerId;
                getTask.TaskActivationDate = taskActivationDate;
                getTask.TaskAcceptedDate = taskAcceptedDate;
                getTask.TaskCompletedBy = taskCompletedBy;
                getTask.TaskEscalatedDate = taskEscalatedDate;
                getTask.TaskOwnerId = taskOwnerId;
                getTask.TaskStatus = taskStatus;
                getTask.TaskTitle = taskTitle;
                getTask.TaskDescription = taskDescription;
                getTask.HasPredecessor = hasPredecessor;
                getTask.TaskSequence = taskSequence;
                getTask.EscalationDuration = escalationDuration;
                getTask.ExpectedCompletionTime = expectedCompletionTime;
                getTask.UpdatedDate = DateTime.Now.GetDateTimeOffset();
                getTask.UpdatedBy = currentUserId;
                _context.Update(getTask);
                await _context.SaveChangesAsync();

                return activeIncidentTaskId;
            }
        }

        return 0;
    }

    public async Task CreateActiveCheckList(int activeIncidentTaskId, int incidentTaskId, int userId,
        string timeZoneId = "GMT Standard Time")
    {
        var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", activeIncidentTaskId);
        var pIncidentTaskID = new SqlParameter("@IncidentTaskID", incidentTaskId);
        var pUserID = new SqlParameter("@UserID", userId);
        var pTimeZone = new SqlParameter("@TimeZoneId", timeZoneId);

        await _context.Database.ExecuteSqlRawAsync(
            "Pro_Create_Active_CheckList @ActiveIncidentTaskID, @IncidentTaskID, @UserID, @TimeZoneId",
            pActiveIncidentTaskID, pIncidentTaskID, pUserID, pTimeZone);
    }

    public async Task CreateTaskRecipient(int activeIncidentId, int activeIncidentTaskId, int incidentTaskId)
    {
        var pActiveIncidentId = new SqlParameter("@IncidentActivationID", activeIncidentId);
        var pActiveIncidentTaskId = new SqlParameter("@ActiveIncidentTaskID", activeIncidentTaskId);
        var pIncidentTaskId = new SqlParameter("@IncidentTaskID", incidentTaskId);

        await _context.Database.ExecuteSqlRawAsync("Pro_Create_Launch_Task_Receipient_List @IncidentActivationID, @ActiveIncidentTaskID, @IncidentTaskID",
            pActiveIncidentId, pActiveIncidentTaskId, pIncidentTaskId);
    }

    private async Task CreateIncidentLocation(AffectedLocation affectedLocation, int activeIncidentId, int companyId)
    {
        var pActiveIncidentId = new SqlParameter("@ActiveIncidentID", activeIncidentId);
        var pCompanyId = new SqlParameter("@CompanyID", companyId);
        var pLocationId = new SqlParameter("@LocationID", affectedLocation.LocationID);
        var pImpactedLocationId = new SqlParameter("@ImpactedLocationID", affectedLocation.ImpactedLocationID);
        var pLocationName = new SqlParameter("@LocationName", affectedLocation.LocationName);
        var pLat = new SqlParameter("@Lat", affectedLocation.Lat);
        var pLng = new SqlParameter("@Lng", affectedLocation.Lng);
        var pAddress = new SqlParameter("@Address", affectedLocation.Address);
        var pLocationType = new SqlParameter("@LocationType", affectedLocation.LocationType);

        await _context.Database.ExecuteSqlRawAsync("Pro_Create_Incident_Location @ActiveIncidentID, @CompanyID, @LocationID, @ImpactedLocationID, @LocationName, @Lat, @Lng, @Address, @LocationType",
            pActiveIncidentId, pCompanyId, pLocationId, pImpactedLocationId, pLocationName, pLat, pLng, pAddress, pLocationType);
    }

    public List<UpdateIncidentStatusReturn> GetCompanyActiveIncident(int CompanyID, int UserID, string Status, int RecordStart = 0, int RecordLength = 100, string SearchString = "", string OrderBy = "Name", string OrderDir = "asc")
    {
        List<UpdateIncidentStatusReturn> result = new();
        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pUserID = new SqlParameter("@UserID", UserID);
            var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
            var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
            var pSearchString = new SqlParameter("@SearchString", SearchString);
            var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
            var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
            var pStatus = new SqlParameter("@Status", Status);

            result = _context.Set<UpdateIncidentStatusReturn>().FromSqlRaw("Pro_Incident_Active @CompanyID, @UserID, @Status, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir",
                pCompanyID, pUserID, pStatus, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir).ToList().Select(c =>
                {
                    c.InitiatedByName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    return c;
                }).ToList();

            return result;
        }
        catch (Exception ex)
        {
        }
        return result;
    }
    public async Task<User?> GetUserById(int id)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(x => x.UserId == id);
    }
    public async Task<List<UserTaskHead>> GetUserTasks(int CurrentUserID)
    {
        try
        {

            var pUserID = new SqlParameter("@UserID", CurrentUserID);

            var task_list = await _context.Set<UserTaskHead>().FromSqlRaw("exec Pro_Get_User_Task @UserID", pUserID).ToListAsync();

            task_list.Select(c => {
                c.LaunchedBy = new UserFullName { Firstname = c.LaunchByFirstName, Lastname = c.LaunchByLastName };
                return c;
            }).ToList();

            UserTaskCount = task_list.Count;
            return task_list;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<TaskIncidentHeader> GetActiveIncidentWorkflow(int activeIncidentID)
    {
        try
        {

            //Get the whole active incident task detail
            TaskIncidentHeader task = await TaskIncidentHeader(activeIncidentID);
            task.LaunchedByName = new UserFullName { Firstname = task.FirstName, Lastname = task.LastName };

            if (task != null)
            {
                return task;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task<TaskIncidentHeader> TaskIncidentHeader(int ActiveIncidentID)
    {
        try
        {
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);

            var result = _context.Set<TaskIncidentHeader>().FromSqlRaw("exec Pro_Get_Task_Incident_Header @ActiveIncidentID", pActiveIncidentID).AsEnumerable();
            var task = result.FirstOrDefault();
            return task;
        }
        catch (Exception ex)
        {
            throw ex;
            // return new TaskIncidentHeader();
        }
    }
    public async Task<List<IncidentTaskDetails>> GetActiveIncidentTasks(int ActiveIncidentID, int ActiveIncidentTaskID, int CompanyID, bool single = false)
    {
        try
        {
            int obj_map_id = (from O in _context.Set<Object>()
                              join OM in _context.Set<ObjectMapping>() on O.ObjectId equals OM.SourceObjectId
                              where O.ObjectTableName == "Group"
                              select OM.ObjectMappingId).FirstOrDefault();

            var pt_type = await _context.Set<TaskParticipantType>().ToListAsync();
            int action_type = pt_type.Where(w => w.TaskParticipantTypeName == "ACTION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();
            int escal_type = pt_type.Where(w => w.TaskParticipantTypeName == "ESCALATION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();
            int reallocate_type = pt_type.Where(w => w.TaskParticipantTypeName == "REALLOCATION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();
            int delegate_type = pt_type.Where(w => w.TaskParticipantTypeName == "DELEGATION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();

            var participent_list = await _active_participants(ActiveIncidentID);

            await FixActiveTaskOrder(ActiveIncidentID);

            //var users = (from U in db.Users where U.CompanyId == CompanyID where U.Status != 2 select U).ToList();

            var TaskList = await _active_incident_tasks(ActiveIncidentID, ActiveIncidentTaskID);
            TaskList.Select(async c => {
                c.TaskDeclinedBy = (from TIA in participent_list
                                    where TIA.ActionStatus == "DECLINED" && TIA.ActiveIncidentTaskID == c.ActiveIncidentTaskID
                                    orderby TIA.ActiveIncidentTaskParticipantID descending
                                    select new DeclinedList { FirstName = TIA.FirstName, LastName = TIA.LastName, TaskDeclinedBy = TIA.ParticipantUserID }).FirstOrDefault();
                c.TaskReallocatedTo = (from RPT in participent_list
                                       where RPT.ActionStatus == "REALLOCATED" && RPT.ActiveIncidentTaskID == c.ActiveIncidentTaskID
                                       orderby RPT.ActiveIncidentTaskParticipantID descending
                                       select new ReallocatedList
                                       {
                                           FirstName = RPT.FirstName,
                                           LastName = RPT.LastName,
                                           UserId = RPT.UserId,
                                           ReallocatedTo = RPT.ParticipantUserID
                                       }).FirstOrDefault();
                c.TaskDelegatedTo = (from RPT in participent_list
                                     where RPT.ActionStatus == "DELEGATED" && RPT.ActiveIncidentTaskID == c.ActiveIncidentTaskID
                                     orderby RPT.ActiveIncidentTaskParticipantID descending
                                     select new DelegatedList { FirstName = RPT.FirstName, LastName = RPT.LastName, DelegatedTo = RPT.ParticipantUserID }).FirstOrDefault();
                c.Recepeints = (from PT in participent_list
                                where PT.ParticipantTypeID == action_type && PT.ActiveIncidentTaskID == c.ActiveIncidentTaskID
                                select PT.ParticipantUserID).ToList();
                c.EscalationRecepeints = (from PT in participent_list
                                          where PT.ParticipantTypeID == escal_type &&
                                          PT.ActiveIncidentTaskID == c.ActiveIncidentTaskID
                                          select PT.ParticipantUserID).ToList();
                c.ReallocateRecepeints = (from PT in participent_list
                                          where PT.ParticipantTypeID == reallocate_type &&
                                          PT.ActiveIncidentTaskID == c.ActiveIncidentTaskID
                                          select PT.ParticipantUserID).ToList();
                c.DelegateRecepeints = (from PT in participent_list
                                        where PT.ParticipantTypeID == delegate_type &&
                                        PT.ActiveIncidentTaskID == c.ActiveIncidentTaskID
                                        select PT.ParticipantUserID).ToList();
                c.TaskPredecessor = await _active_incident_tasks_predeccessor(c.ActiveIncidentTaskID, c.ActiveIncidentID);
                c.TaskSuccessor = await _active_incident_tasks_successor(c.ActiveIncidentTaskID, c.ActiveIncidentID);
                return c;
            });

            if (single)
            {
                return (from T in TaskList where T.ActiveIncidentTaskID == ActiveIncidentTaskID select T).ToList();
            }
            return TaskList;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    public async Task<List<IncidentTaskDetails>> _active_incident_tasks(int ActiveIncidentID, int ActiveIncidentTaskID)
    {
        try
        {
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
            var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);

            var result = await _context.Set<IncidentTaskDetails>().FromSqlRaw("exec Pro_Get_Active_Incident_Task @ActiveIncidentID, @ActiveIncidentTaskID",
                pActiveIncidentID, pActiveIncidentTaskID).ToListAsync();


            result.Select(c => {
                c.TaskOwner = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                return c;
            }).ToList();

            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<List<ActiveTaskParticiants>> _active_participants(int ActiveIncidentID, int ActiveTaskID = 0, string RecipientType = "")
    {
        try
        {
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);
            var pActiveTaskID = new SqlParameter("@ActiveTaskID", ActiveTaskID);
            var pRecipientType = new SqlParameter("@RecipientType", RecipientType);

            var result = await _context.Set<ActiveTaskParticiants>().FromSqlRaw("exec Pro_Get_Active_Task_Participant @ActiveIncidentID, @ActiveTaskID, @RecipientType",
                pActiveIncidentID, pActiveTaskID, pRecipientType).ToListAsync();

            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task FixActiveTaskOrder(int ActiveIncidentID)
    {
        try
        {
            var rearrange_tasks = await _context.Set<TaskActiveIncident>()
                                   .Where(IT => IT.ActiveIncidentId == ActiveIncidentID)
                                   .OrderBy(IT => IT.TaskSequence).ToListAsync();
            int newseq = 1;
            foreach (var rtask in rearrange_tasks)
            {
                if (rtask.TaskSequence != newseq)
                    rtask.TaskSequence = newseq;
                newseq++;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<TaskPredecessorList>> _active_incident_tasks_predeccessor(int ActiveIncidentTaskID, int ActiveIncidentID)
    {
        try
        {

            var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);

            var result = await _context.Set<TaskPredecessorList>().FromSqlRaw("exec Pro_Get_Active_Task_Predeccessor @ActiveIncidentTaskID, @ActiveIncidentID",
                pActiveIncidentTaskID, pActiveIncidentID).ToListAsync();

            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<List<TaskPredecessorList>> _active_incident_tasks_successor(int ActiveIncidentTaskID, int ActiveIncidentID)
    {
        try
        {

            var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);

            var result = await _context.Set<TaskPredecessorList>().FromSqlRaw("exec Pro_Get_Active_Task_Successor @ActiveIncidentTaskID, @ActiveIncidentID",
                pActiveIncidentTaskID, pActiveIncidentID).ToListAsync();

            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<TaskActiveIncident> GetTaskActiveIncidentById(int ActiveIncidentTaskID, int CompanyID)
    {
        var task = await _context.Set<TaskActiveIncident>().Where(AIT => AIT.ActiveIncidentTaskId == ActiveIncidentTaskID && AIT.CompanyId == CompanyID).FirstOrDefaultAsync();
        return task;
    }
    public async Task<List<TaskIncidentAction>> GetTaskIncidentActionByDeline(int ActiveIncidentTaskID)
    {
        var check_declined = await _context.Set<TaskIncidentAction>().Where(TA => TA.TaskActionTypeId == 3 && TA.ActiveIncidentTaskId == ActiveIncidentTaskID).ToListAsync();

        return check_declined;
    }

    public async Task send_notifiation_to_groups(List<string> GroupType, int ActiveIncidentID, int ActiveIncidentTaskID,
           string Message, int CurrentUserID, int CompanyID, string TimeZoneId, bool IncludeKeyContact = true,
           int Source = 1, int[] MessageMethod = null, List<NotificationUserList> UserToNotify = null, int CascadePlanID = 0, string sourceAction = "")
    {
        try
        {

            bool IncludeActionList = false;
            bool IncludeEscalationList = false;
            bool IncludeNotificationList = false;

            foreach (string grp in GroupType)
            {
                if (grp.ToUpper() == "ACTION")
                    IncludeActionList = true;
                if (grp.ToUpper() == "ESCALATION")
                    IncludeEscalationList = true;
                if (grp.ToUpper() == "INCIDENT")
                    IncludeNotificationList = true;
            }

            Messaging MSG = new Messaging(_context, _httpContextAccessor);
            MSG.TimeZoneId = TimeZoneId;
            MSG.CascadePlanID = CascadePlanID;
            MSG.MessageSourceAction = sourceAction;
            int tblmessageid = await MSG.CreateMessage(CompanyID, Message, "Incident", ActiveIncidentID, 999, CurrentUserID,
                Source, DateTime.Now.GetDateTimeOffset(TimeZoneId), false, null, 99, 0, ActiveIncidentTaskID, false, false, MessageMethod);


            if (tblmessageid > 0)
            {
                if (UserToNotify != null)
                {

                    foreach (NotificationUserList lUsr in UserToNotify)
                    {
                        await MSG.CreateMessageList(tblmessageid, lUsr.UserId, lUsr.IsTaskRecipient, MSG.TextUsed, MSG.PhoneUsed, MSG.EmailUsed, MSG.PushUsed, CurrentUserID, TimeZoneId);
                    }
                    await QueueConsumer.CreateMessageList(tblmessageid);
                    IsFundAvailable = QueueConsumer.IsFundAvailable;
                }

                //Create participent list

                var pIncidentActivationId = new SqlParameter("@IncidentActivationID", ActiveIncidentID);
                var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);
                var pMessageId = new SqlParameter("@MessageID", tblmessageid);
                var pIncludeKeyContact = new SqlParameter("@IncludeKeyContact", IncludeKeyContact);
                var pIncludeActionList = new SqlParameter("@IncludeActionList", IncludeActionList);
                var pIncludeEscalationList = new SqlParameter("@IncludeEscalationList", IncludeEscalationList);
                var pIncludeNotificationList = new SqlParameter("@IncludeNotificationList", IncludeNotificationList);
                var pCustomerTime = new SqlParameter("@CustomerTime", DateTime.Now.GetDateTimeOffset(TimeZoneId));
                var pCurrentUserID = new SqlParameter("@CurrentUserID", CurrentUserID);

                var RowsCount = await _context.Set<Result>().FromSqlRaw("exec Pro_Task_Update_Message_List @IncidentActivationID,@ActiveIncidentTaskID,@MessageID,@IncludeKeyContact,@IncludeActionList,@IncludeEscalationList,@IncludeNotificationList,@CustomerTime,@CurrentUserID",
                       pIncidentActivationId, pActiveIncidentTaskID, pMessageId, pIncludeKeyContact, pIncludeActionList, pIncludeEscalationList, pIncludeNotificationList, pCustomerTime, pCurrentUserID).FirstOrDefaultAsync();

                IsFundAvailable = await MSG.CalculateMessageCost(CompanyID, tblmessageid, Message);

                Task.Factory.StartNew(() => QueueHelper.MessageDeviceQueue(tblmessageid, "Incident", 1, CascadePlanID));
                //QueueHelper.MessageDevicePublish(tblmessageid, 1);

                QueueConsumer.CreateCascadingJobs(CascadePlanID, tblmessageid, ActiveIncidentID, CompanyID, TimeZoneId);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<int> AddTaskAction(int ActiveIncidentTaskID, string ActionDescription, int CurrentUserID, int TaskActionTypeID, string TimeZoneId)
    {
        try
        {
            TaskIncidentAction TIA = new TaskIncidentAction
            {
                ActionDate = DateTime.Now.GetDateTimeOffset(TimeZoneId),
                ActionDescription = ActionDescription,
                ActiveIncidentTaskId = ActiveIncidentTaskID,
                TaskActionBy = CurrentUserID,
                TaskActionTypeId = TaskActionTypeID
            };
            await _context.AddAsync(TIA);
            await _context.SaveChangesAsync();
            return TIA.IncidentTaskActionId;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task notify_users(int ActiveIncidentID, int ActiveIncidentTaskID, List<NotificationUserList> UserToNotify, string Message, int CurrentUserID,
        int CompanyID, string TimeZoneId, bool IncludeKeyContact = true, int Source = 1, int[] MessageMetod = null, int CascadePlanID = 0)
    {
        try
        {
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor); ;
            List<NotificationUserList> TaskPtcpntList = new List<NotificationUserList>();
            if (IncludeKeyContact)
            {
                var KeyContacts = await _context.Set<ActiveIncidentKeyContact>().Where(KC => KC.IncidentActivationId == ActiveIncidentID).Select(KC => new { KC.UserId }).ToListAsync();

                foreach (var Keycon in KeyContacts)
                {
                    TaskPtcpntList.Add(new NotificationUserList(Keycon.UserId, false));
                }
            }

            TaskPtcpntList = await DBC.GetUniqueUsers(TaskPtcpntList, UserToNotify);

            Messaging MSG = new Messaging(_context, _httpContextAccessor);
            MSG.TimeZoneId = TimeZoneId;
            MSG.CascadePlanID = CascadePlanID;
            MSG.MessageSourceAction = MessageSourceAction;

            int tblmessageid = await MSG.CreateMessage(CompanyID, Message, "Incident", ActiveIncidentID, 999, CurrentUserID, Source,
                       DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId), false, null, 99, 0, ActiveIncidentTaskID, false, false, MessageMetod);

            if (tblmessageid > 0)
            {
                foreach (NotificationUserList lUsr in TaskPtcpntList)
                {
                    await MSG.CreateMessageList(tblmessageid, lUsr.UserId, lUsr.IsTaskRecipient, MSG.TextUsed, MSG.PhoneUsed, MSG.EmailUsed, MSG.PushUsed, CurrentUserID, TimeZoneId);
                }
                await QueueConsumer.CreateMessageList(tblmessageid);
                IsFundAvailable = QueueConsumer.IsFundAvailable;
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<List<GetAllUser>> GetTaskUserList(int Start, int Length, Search Search, string TypeName, int ActiveIncidentTaskID, string CompanyKey, int OutLoginUserId, int OutUserCompanyId)
    {
        var RecordStart = Start == 0 ? 0 : Start;
        var RecordLength = Length == 0 ? int.MaxValue : Length;
        var SearchString = !string.IsNullOrEmpty(Search.Value) ? Search.Value : "";


        var pCompanyId = new SqlParameter("@CompanyId", OutUserCompanyId);
        var pUserId = new SqlParameter("@UserID", OutLoginUserId);
        var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);
        var pTypeName = new SqlParameter("@TypeName", TypeName);
        var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
        var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
        var pSearchString = new SqlParameter("@SearchString", SearchString);
        var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

        //TODODONE - filter the users based on the parameter and user profile
        var MainUserlist = await _context.Set<GetAllUser>().FromSqlRaw(" exec Pro_Get_Task_User_SelectAll @CompanyId,@ActiveIncidentTaskID, @TypeName, @UserID, @RecordStart, @RecordLength, @SearchString,@UniqueKey",
            pCompanyId, pActiveIncidentTaskID, pTypeName, pUserId, pRecordStart, pRecordLength, pSearchString, pUniqueKey)
            .OrderBy(o => o.FirstName).ToListAsync();
        return MainUserlist;
    }
    public async Task<List<NotificationUserList>> GetActiveParticipantList(int IncidentActivationId, string GroupType = "ACTION", int ActiveIncidentTaskID = 0, bool IsTaskRecipient = true)
    {
        List<NotificationUserList> PTList = new List<NotificationUserList>();

        //Use: EXEC [dbo].[Pro_ActiveIncidentTask_GetAllActiveParticipantList] @IncidentActivationID,@GroupType,@ActiveIncidentTaskID,@IsTaskRecipient
        try
        {
            //Get Object Mapping Id of the group object
            int obj_map_id = (from O in _context.Set<Object>()
                              join OM in _context.Set<ObjectMapping>() on O.ObjectId equals OM.SourceObjectId
                              where O.ObjectTableName == "Group"
                              select OM.ObjectMappingId).FirstOrDefault();

            //Get the group type of of the participant to fetch
            int pt_type = await _context.Set<TaskParticipantType>()
                           .Where(PT => PT.TaskParticipantTypeName == GroupType)
                           .Select(PT => PT.TaskParticipantTypeId).FirstOrDefaultAsync();

            //Get all the participant list from incident participant table
            var pt_list = await _context.Set<TaskActiveIncident>().Include(ty => ty.TaskActiveIncidentParticipant)
                           .Where(IT => IT.ActiveIncidentId == IncidentActivationId && (IT.TaskActiveIncidentParticipant.ParticipantTypeId == pt_type || IT.TaskActiveIncidentParticipant.ActionStatus == "UNALLOCATED"))
                           .ToListAsync();

            if (ActiveIncidentTaskID > 0)
            {
                pt_list = pt_list.Where(NL => NL.TaskActiveIncidentParticipant.ActiveIncidentTaskId == ActiveIncidentTaskID).ToList();
            }
            //Fetch the user list of the groups provided in incidenttask participant
            var GroupUserList = (from TIP in pt_list
                                 join OM in _context.Set<ObjectRelation>() on new { SourceID = TIP.TaskActiveIncidentParticipant.ParticipantGroupId, ObjectMappingId = obj_map_id }
                                 equals new { SourceID = OM.SourceObjectPrimaryId, ObjectMappingId = OM.ObjectMappingId }
                                 where TIP.ActiveIncidentId == IncidentActivationId && TIP.TaskActiveIncidentParticipant.ParticipantGroupId > 0
                                 select new NotificationUserList(OM.TargetObjectPrimaryId, IsTaskRecipient = IsTaskRecipient)).ToList();

            //Get the lsit of individual user from incident task participant
            var UserList = pt_list
                            .Where(TIP => TIP.TaskActiveIncidentParticipant.ParticipantUserId > 0)
                            .Select(IT => new NotificationUserList(IT.TaskActiveIncidentParticipant.ParticipantUserId, IsTaskRecipient)).ToList();

            PTList = GroupUserList.Union(UserList).Distinct().ToList();
            return PTList;
        }
        catch (Exception ex)
        {
            throw ex;
            return PTList;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ActiveIncidentTaskID"></param>
    /// <param name="TaskActionReason"></param>
    /// <param name="CurrentUserID"></param>
    /// <param name="CompanyID"></param>
    /// <param name="TimeZoneId"></param>
    /// <returns></returns>

    public async Task<bool> check_for_last_member(int ActiveIncidentID, int ActiveIncidentTaskID, string Group)
    {
        try
        {
            List<NotificationUserList> UList = await GetActiveParticipantList(ActiveIncidentID, Group, ActiveIncidentTaskID);
            if (UList.Count == 1)
                return true;

            int rejected_list = await _context.Set<TaskIncidentAction>().Where(TA => TA.ActiveIncidentTaskId == ActiveIncidentTaskID && TA.TaskActionTypeId == 3).CountAsync();

            if (UList.Count == rejected_list)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ActiveIncidentTaskID"></param>
    /// <param name="TaskActionReason"></param>
    /// <param name="ReallocateTo"></param>
    /// <param name="CurrentUserID"></param>
    /// <param name="CompanyID"></param>
    /// <param name="TimeZoneId"></param>
    /// <returns></returns>
    public async Task<TaskActiveIncident> ReallocateTask(int ActiveIncidentTaskID, string TaskActionReason, int ReallocateTo, int[] MessageMethod, int CascadePlanID, int CurrentUserID, int CompanyID, string TimeZoneId)
    {
        try
        {
            MessageSourceAction = SourceAction.TaskReallocated;
            string Message = "";
            var task = await _context.Set<TaskActiveIncident>().Where(AIT => AIT.ActiveIncidentTaskId == ActiveIncidentTaskID && AIT.CompanyId == CompanyID).FirstOrDefaultAsync();

            if (task != null)
            {

                if (new List<int> { 1, 7 }.Contains(task.TaskStatus))
                {
                    Message = "Cannot reallocated this task.";

                }
                else if (task.TaskOwnerId != CurrentUserID)
                {

                    Message = "Cannot Rellocate! You are no longer the owner of the task";

                }

                int pt_type = await _context.Set<TaskParticipantType>().Where(PT => PT.TaskParticipantTypeName == "REALLOCATION").Select(PT => PT.TaskParticipantTypeId).FirstOrDefaultAsync();

                await change_participant_type(ReallocateTo, ActiveIncidentTaskID, pt_type, "REALLOCATED");

                task.TaskStatus = 4;
                task.UpdatedDate = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                task.UpdatedBy = CurrentUserID;
                await _context.SaveChangesAsync();

                var action_user = await _context.Set<User>().Where(U => U.UserId == ReallocateTo).FirstOrDefaultAsync();

                string username = "";
                if (action_user != null)
                {
                    username = action_user.FirstName + " " + action_user.LastName;
                }

                //Add task action history
                await AddTaskAction(ActiveIncidentTaskID, "Task reallocated to " + username + "<br/>Comment:" + TaskActionReason, CurrentUserID, task.TaskStatus, TimeZoneId);

                List<NotificationUserList> TaskPtcpntList = new List<NotificationUserList>();
                TaskPtcpntList.Add(new NotificationUserList(ReallocateTo, true));
                string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 20) + "\" is reassigned to " + username + ". " + Environment.NewLine + " with comment: " + TaskActionReason;

                //bool NotifyKeyContact = false;
               // bool.TryParse(_DBC.GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", CompanyID), out NotifyKeyContact);

                await notify_users(task.ActiveIncidentId, task.ActiveIncidentTaskId, TaskPtcpntList, action_update, CurrentUserID, CompanyID, TimeZoneId, false, 3, MessageMethod, CascadePlanID);

                return task;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ActiveIncidentTaskID"></param>
    /// <param name="TaskActionReason"></param>
    /// <param name="DelegateTo"></param>
    /// <param name="CurrentUserID"></param>
    /// <param name="CompanyID"></param>
    /// <param name="TimeZoneId"></param>
    /// <returns></returns>
    public async Task<TaskActiveIncident> DelegateTask(int ActiveIncidentTaskID, string TaskActionReason, int[] DelegateTo, int[] MessageMethod, int CascadePlanID, int CurrentUserID, int CompanyID, string TimeZoneId)
    {
        try
        {
            string Message = string.Empty;
            MessageSourceAction = SourceAction.TaskDelegated;
            var task = await _context.Set<TaskActiveIncident>().Where(AIT => AIT.ActiveIncidentTaskId == ActiveIncidentTaskID && AIT.CompanyId == CompanyID).FirstOrDefaultAsync();

            if (task != null)
            {

                if (task.TaskOwnerId != CurrentUserID)
                {

                    Message = "Cannot Deletegate!. You are no longer the owner of the task";

                }

                int pt_type = await _context.Set<TaskParticipantType>().Where(PT => PT.TaskParticipantTypeName == "DELEGATION").Select(PT => PT.TaskParticipantTypeId).FirstOrDefaultAsync();

                task.TaskStatus = 5;
                task.UpdatedDate = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                task.UpdatedBy = CurrentUserID;
                _context.Update(task);
                await _context.SaveChangesAsync();

                List<NotificationUserList> TaskPtcpntList = new List<NotificationUserList>();
                string username = "";
                string delim = "";
                foreach (int delegatee in DelegateTo)
                {
                    await change_participant_type(delegatee, ActiveIncidentTaskID, pt_type, "DELEGATED");
                    var action_user = await _context.Set<User>().Where(U => U.UserId == delegatee).FirstOrDefaultAsync();
                    if (action_user != null)
                    {
                        username += delim + action_user.FirstName + " " + action_user.LastName;
                        delim = ", ";
                    }
                    TaskPtcpntList.Add(new NotificationUserList(delegatee, true));
                }

                //Add task action history
                await AddTaskAction(ActiveIncidentTaskID, "Task delegated to " + username + "<br/>Comment:" + TaskActionReason, CurrentUserID, task.TaskStatus, TimeZoneId);

                string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 20) + "\" delegated to " + username + ". " + Environment.NewLine + " With comment: " + TaskActionReason;

                //bool NotifyKeyContact = false;
                //bool.TryParse(DBC.GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", CompanyID), out NotifyKeyContact);

                await notify_users(task.ActiveIncidentId, task.ActiveIncidentTaskId, TaskPtcpntList, action_update, CurrentUserID, CompanyID, TimeZoneId, false, 3, MessageMethod, CascadePlanID);

                return task;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ActiveIncidentTaskID"></param>
    /// <param name="TaskActionReason"></param>
    /// <param name="DelegateTo"></param>
    /// <param name="CurrentUserID"></param>
    /// <param name="CompanyID"></param>
    /// <param name="TimeZoneId"></param>
    /// <returns></returns>
    public async Task<TaskActiveIncident> ReassignTask(int ActiveIncidentTaskID, int[] ActionUsers, int[] EscalationUsers,
        string TaskActionReason, bool RemoveCurrentOwner, int CurrentUserID, int CompanyID, string TimeZoneId)
    {
        try
        {
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
            MessageSourceAction = SourceAction.TaskReassigned;
            var task = await GetTaskActiveIncidentById(ActiveIncidentTaskID, CompanyID);
            if (task != null)
            {
                List<NotificationUserList> TaskPtcpntList = new List<NotificationUserList>();

                if (ActionUsers.Length > 0)
                {
                    int act_type = await _context.Set<TaskParticipantType>().Where(PT => PT.TaskParticipantTypeName == "ACTION").Select(PT => PT.TaskParticipantTypeId).FirstOrDefaultAsync();
                    foreach (int ActUser in ActionUsers)
                    {
                        await change_participant_type(ActUser, ActiveIncidentTaskID, act_type, "UNALLOCATED", true);

                        if (task.TaskActivationDate.Year > 2000) //only add when the task is started
                            TaskPtcpntList.Add(new NotificationUserList(ActUser, true));
                    }
                }

                if (EscalationUsers.Length > 0)
                {
                    int esc_type = await _context.Set<TaskParticipantType>().Where(PT => PT.TaskParticipantTypeName == "ESCALATION").Select(PT => PT.TaskParticipantTypeId).FirstOrDefaultAsync();
                    foreach (int EcsUser in EscalationUsers)
                    {
                        await change_participant_type(EcsUser, ActiveIncidentTaskID, esc_type, "UNALLOCATED", true);

                        if (task.TaskActivationDate.Year > 2000) //only add when the task is started
                            TaskPtcpntList.Add(new NotificationUserList(EcsUser, true));
                    }
                }

                if (RemoveCurrentOwner)
                    task.TaskOwnerId = 0;

                task.TaskStatus = 1;
                
                await _context.SaveChangesAsync();


                //Add task action history
                await AddTaskAction(ActiveIncidentTaskID, "Reassigned after review<br/>Comment:" + TaskActionReason, CurrentUserID, task.TaskStatus, TimeZoneId);

                string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 20) + "\" is available for action." + Environment.NewLine + " Comment: " + TaskActionReason;

                bool NotifyKeyContact = false;
                bool.TryParse(await GetCompanyParameter("INC_UPDATE_GROUP_NOTIFY_KEYCONTACTS", CompanyID), out NotifyKeyContact);
                await notify_users(task.ActiveIncidentId, task.ActiveIncidentTaskId, TaskPtcpntList, action_update, CurrentUserID, CompanyID, TimeZoneId, NotifyKeyContact, 3);

                return task;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return null;
    }

    public async Task<string> GetCompanyParameter(string key, int companyId, string @default = "",
            string customerId = "")
    {

        key = key.ToUpper();

        if (companyId > 0)
        {
            var lkp = await _context.Set<CompanyParameter>()
                .FirstOrDefaultAsync(x => x.Name == key && x.CompanyId == companyId);

            if (lkp != null)
            {
                @default = lkp.Value;
            }
            else
            {
                var lpr = await _context.Set<LibCompanyParameter>()
                    .FirstOrDefaultAsync(x => x.Name == key);

                @default = lpr != null ? lpr.Value : await LookupWithKey(key, @default);
            }
        }

        if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(key))
        {
            var cmp = await _context.Set<Company>().FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (cmp != null)
            {
                var lkp = await _context.Set<CompanyParameter>()
                    .FirstOrDefaultAsync(x => x.Name == key && x.CompanyId == cmp.CompanyId);
                if (lkp != null)
                {
                    @default = lkp.Value;
                }
            }
            else
            {
                @default = "NOT_EXIST";
            }
        }

        return @default;
    }

    public async Task<string> LookupWithKey(string Key, string Default = "")
    {
        try
        {
            var LKP = await _context.Set<SysParameter>()
                       .Where(L => L.Name == Key
                       ).FirstOrDefaultAsync();
            if (LKP != null)
            {
                Default = LKP.Value;
            }
            return Default;
        }
        catch (Exception ex)
        {
            return Default;
        }
    }
    public async Task CompleteAllTask(int ActiveIncidentID, int CurrentUserID, int CompanyID, string TimeZoneId)
    {
        try
        {
            DBCommon DBC = new DBCommon(_context, _httpContextAccessor);
            var tasks = await _context.Set<TaskActiveIncident>()
                         .Where(AIT => AIT.ActiveIncidentId == ActiveIncidentID && AIT.TaskStatus != 7 && AIT.CompanyId == CompanyID)
                         .ToListAsync();
            //Get Action By username
            var get_user = await _context.Set<User>().Where(U => U.UserId == CurrentUserID).FirstOrDefaultAsync();
            string username = "";
            if (get_user != null)
            {
                username = get_user.FirstName + " " + get_user.LastName;
            }
            foreach (var task in tasks)
            {
                try
                {
                    task.TaskStatus = 7;
                    task.UpdatedDate = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    task.UpdatedBy = CurrentUserID;
                    task.TaskCompletedBy = CurrentUserID;
                    _context.Update(task);
                    await _context.SaveChangesAsync();

                    string task_action = "Task completed forcefully by " + username + " upon closing the incident.";
                    await AddTaskAction(task.ActiveIncidentTaskId, task_action, CurrentUserID, task.TaskStatus, TimeZoneId);

                    //Delete Scheduled jobs for the incident
                    DBC.DeleteScheduledJob("START_ACPT_TASK_" + task.ActiveIncidentTaskId, "TASK_SCHEDULE");
                    DBC.DeleteScheduledJob("START_ESCL_TASK_" + task.ActiveIncidentTaskId, "TASK_SCHEDULE");

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Incident task actions extended functions
    /// </summary>
    /// <param name="ActiveIncidentTaskID"></param>
    public async Task remove_old_delegates(int ActiveIncidentTaskID)
    {
        try
        {
            var pt = await _context.Set<TaskActiveIncidentParticipant>()
                      .Where(PT => PT.ActiveIncidentTaskId == ActiveIncidentTaskID && PT.ActionStatus == "DELEGATED"
                      ).FirstOrDefaultAsync();
            if (pt != null)
            {
                int prev_type_id = pt.PreviousParticipantTypeId;

                if (prev_type_id > 0)
                {
                    pt.ParticipantTypeId = prev_type_id;
                }
                else
                {
                    _context.Remove(pt);
                }
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task change_participant_type(int UserID, int ActiveIncidentTaskID, int NewTypeId, string ActionStatus, bool AddNew = false)
    {
        try
        {
            var participant = await _context.Set<TaskActiveIncidentParticipant>()
                               .Where(TAIP => TAIP.ActiveIncidentTaskId == ActiveIncidentTaskID && TAIP.ParticipantUserId == UserID
                               ).FirstOrDefaultAsync();
            if (participant != null && AddNew == false)
            {
                int current_pt_type = participant.ParticipantTypeId;

                if (NewTypeId > 0)
                {
                    participant.ParticipantTypeId = NewTypeId;
                    participant.PreviousParticipantTypeId = current_pt_type;
                }

                participant.ActionStatus = ActionStatus;
                _context.Update(participant);
                await _context.SaveChangesAsync();
            }
            else
            {
                await create_active_participant_list(UserID, ActiveIncidentTaskID, NewTypeId, "USER", ActionStatus);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// create active incident participant list in database
    /// </summary>
    /// <param name="ObjectId"></param>
    /// <param name="ActiveIncidentTaskID"></param>
    /// <param name="PaticipentTypeId"></param>
    /// <param name="objtype"></param>
    /// <param name="ActionStatus"></param>
    public async Task create_active_participant_list(int ObjectId, int ActiveIncidentTaskID, int PaticipentTypeId, string objtype, string ActionStatus = "UNALLOCATED")
    {
        try
        {
            if (ActiveIncidentTaskID > 0 && PaticipentTypeId > 0)
            {
                TaskActiveIncidentParticipant ITP = new TaskActiveIncidentParticipant();
                ITP.ActiveIncidentTaskId = ActiveIncidentTaskID;
                ITP.ParticipantTypeId = PaticipentTypeId;
                ITP.ParticipantUserId = (objtype == "USER" ? ObjectId : 0);
                ITP.ParticipantGroupId = (objtype == "GROUP" ? ObjectId : 0);
                ITP.ActionStatus = ActionStatus;
                await _context.AddAsync(ITP);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void CreateTaskAcceptedJob(int ActiveIncidentTaskID, DateTimeOffset TaskAccepted, double ExplectedCompltionDuration, string TimeZoneId)
    {
        try
        {
            Quartz.ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
            Quartz.IScheduler sched = schedulerFactory.GetScheduler().Result;

            string jobName = "START_ACPT_TASK_" + ActiveIncidentTaskID;
            string taskTrigger = "START_ACPT_TASK_" + ActiveIncidentTaskID;

            var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "TASK_SCHEDULE", typeof(TaskScheduleJob));
            jobDetail.JobDataMap["ActiveIncidentTaskId"] = ActiveIncidentTaskID;
            jobDetail.JobDataMap["ACTION"] = "COMPLETION";

            //DateTimeOffset taskStartTime = DateTime.Now.AddDays(-2);
            DateTimeOffset taskStartTime = TaskAccepted.AddMinutes(ExplectedCompltionDuration);
            //taskStartTime = DBC.GetServerTime(TimeZoneId, taskStartTime).ToUniversalTime();

            Quartz.ISimpleTrigger trigger = (Quartz.ISimpleTrigger)Quartz.TriggerBuilder.Create()
                                                      .WithIdentity(taskTrigger, "TASK_SCHEDULE")
                                                      .StartAt(taskStartTime)
                                                      .ForJob(jobDetail)
                                                      .Build();

            sched.ScheduleJob(jobDetail, trigger);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task CreatePredecessorJobs(int ActiveIncidentID, int IncidentTaskID, int CurrentUserID, string TimeZoneId)
    {
        try
        {
            var tasklist = (from SU in _context.Set<TaskActiveIncidentPredecessor>()
                            join AIT in _context.Set<TaskActiveIncident>() on SU.ActiveIncidentTaskId equals AIT.ActiveIncidentTaskId
                            where SU.PredecessorTaskId == IncidentTaskID && AIT.ActiveIncidentId == ActiveIncidentID
                            select new { SU, AIT }).Distinct().ToList();

            foreach (var task in tasklist)
            {
                //Get all predecessor task and check if all of them are completed
                // If completed start the escalation event for success task
                bool all_task_completed = true;
                var get_pd_list = (from PD in _context.Set<TaskActiveIncidentPredecessor>()
                                   join IT in _context.Set<TaskActiveIncident>() on PD.PredecessorTaskId equals IT.IncidentTaskId
                                   where PD.ActiveIncidentTaskId == task.AIT.ActiveIncidentTaskId && IT.ActiveIncidentId == ActiveIncidentID
                                   && IT.TaskStatus != 7
                                   select IT.TaskStatus).Any();
                if (get_pd_list)
                {
                    all_task_completed = false;
                }
                else
                {
                    DateTimeOffset ActivationDate = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    task.AIT.TaskActivationDate = ActivationDate;
                    task.AIT.UpdatedDate = ActivationDate;
                    task.AIT.UpdatedBy = CurrentUserID;
                    _context.Update(task);
                    await _context.SaveChangesAsync();

                    List<string> grp = new List<string>();
                    if (task.AIT.TaskStatus == 1)
                    {
                        grp.Add("ACTION");
                        string action_update = "Task " + task.AIT.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.AIT.TaskTitle, 50) + "\" is now available.";
                        await send_notifiation_to_groups(grp, task.AIT.ActiveIncidentId, task.AIT.ActiveIncidentTaskId, action_update, CurrentUserID, task.AIT.CompanyId, TimeZoneId, false, 3, sourceAction: SourceAction.TaskAvailable);
                    }

                }

                if (all_task_completed)
                {
                    CreateTaskEscalationJob(task.AIT.ActiveIncidentTaskId, task.AIT.TaskActivationDate, task.AIT.EscalationDuration, TimeZoneId);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void CreateTaskEscalationJob(int ActiveIncidentTaskID, DateTimeOffset TaskActivation, double EscalationDuration, string TimeZone)
    {
        try
        {
            Quartz.ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
            Quartz.IScheduler sched = schedulerFactory.GetScheduler().Result;

            string jobName = "START_ESCL_TASK_" + ActiveIncidentTaskID;
            string taskTrigger = "START_ESCL_TASK_" + ActiveIncidentTaskID;

            var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "TASK_SCHEDULE", typeof(TaskScheduleJob));
            jobDetail.JobDataMap["ActiveIncidentTaskID"] = ActiveIncidentTaskID;
            jobDetail.JobDataMap["ACTION"] = "ESCALATION";

            DateTimeOffset taskStartTime = TaskActivation.AddMinutes(EscalationDuration);
            //taskStartTime = DBC.GetServerTime(TimeZone, taskStartTime).ToUniversalTime();

            Quartz.ISimpleTrigger trigger = (Quartz.ISimpleTrigger)Quartz.TriggerBuilder.Create()
                                                      .WithIdentity(taskTrigger, "TASK_SCHEDULE")
                                                      .StartAt(taskStartTime)
                                                      .ForJob(jobDetail)
                                                      .Build();

            sched.ScheduleJob(jobDetail, trigger);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<TaskActiveIncidentParticipant> GetTaskActiveIncidentParticipantById(int ActiveIncidentTaskID, int CurrentUserID)
    {
        var pType = await _context.Set<TaskActiveIncidentParticipant>()
                            .Where(PT => PT.ActiveIncidentTaskId == ActiveIncidentTaskID &&
                             PT.ParticipantUserId == CurrentUserID).FirstOrDefaultAsync();
        return pType;
    }
    public async Task<int> GetTaskActiveIncidentParticipantIdByStatus(int ActiveIncidentTaskID)
    {
        var pType = await _context.Set<TaskActiveIncidentParticipant>()
                            .Where(PT => PT.ActiveIncidentTaskId == ActiveIncidentTaskID &&
                                                 PT.ActionStatus == "DELEGATED").Select(a => a.ParticipantUserId).FirstOrDefaultAsync();
        return pType;
    }

    public async Task<int> UpdateTaskActiveIncident(TaskActiveIncident task)
    {
        _context.Update(task);
        await _context.SaveChangesAsync();
        return task.ActiveIncidentTaskId;
    }
    public async Task<List<TaskAssignedUser>> GetTaskAssignedUsers(int ActiveIncidentTaskID, string TypeName, int CompanyID)
    {
        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pTypeName = new SqlParameter("@TypeName", TypeName);
            var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);
            var pt_list = await _context.Set<TaskAssignedUser>().FromSqlRaw("exec Pro_ActiveIncidentTask_GetTaskAssignedUsers @ActiveIncidentTaskID,@TypeName,@CompanyID", pActiveIncidentTaskID, pTypeName, pCompanyID).ToListAsync();

            return pt_list;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<ActiveCheckList>> GetActiveTaskCheckList(int ActiveIncidentTaskID, int CompanyID, int UserID)
    {
        try
        {

            var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pUserID = new SqlParameter("@UserID", UserID);

            var qresult = await _context.Set<JsonResult>().FromSqlRaw("Pro_Get_Active_TaskCheckList @ActiveIncidentTaskID, @CompanyID, @UserID",
                pActiveIncidentTaskID, pCompanyID, pUserID).FirstOrDefaultAsync();

            if (qresult.Result != null)
            {
                var result = JsonConvert.DeserializeObject<List<ActiveCheckList>>(qresult.Result);
                //return result;
                var newresult = result.ToList().Select(c => {
                    if (c.UserResponse != null)
                    {
                        foreach (UsrResponse UR in c.UserResponse)
                        {
                            var user = _context.Set<User>().Where(w => w.UserId == UR.CreatedBy).FirstOrDefault();
                            UR.FirstName = user.FirstName;
                            UR.LastName = user.LastName;
                        }
                    }
                    return c;
                }).ToList();

                return newresult;
            }
            return null;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<TaskAudit>> GetTaskAudit(int ActiveIncidentTaskID)
    {
        try
        {
            var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);

            var AuditData = await _context.Set<TaskAudit>().FromSqlRaw("exec Pro_Get_Task_Audit @ActiveIncidentTaskID", pActiveIncidentTaskID).ToListAsync();


            AuditData.Select(c => {
                c.ActionBy = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                return c;
            });
            return AuditData;

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    public async Task<dynamic> GetTaskDetails(int ActiveIncidentTaskID, int CompanyID)
    {
        try
        {
           
            var pActiveIncidentTaskID = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);
            
            var participants = _context.Set<ActiveTaskParticiants>().FromSqlRaw("EXEC Pro_ActiveIncidentTask_GetActiveIncidenTasktParticipantList @ActiveIncidentTaskID", pActiveIncidentTaskID).AsEnumerable();

            var action_participants = participants.Where(w => w.ParticipantTypeID == 1).ToList();
            var escalation_participants = participants.Where(w => w.ParticipantTypeID == 2).ToList();
            var task =  _context.Set<TaskActiveIncident>().Include(x => x.IncidentActivation)
                        .Where(AT => AT.ActiveIncidentTaskId == ActiveIncidentTaskID && AT.CompanyId == CompanyID)
                        .Select(AT => new
                        {
                            AT,
                            AT.IncidentActivation.LaunchedOn,
                            AT.IncidentActivation.Name,
                            AT.IncidentActivation.IncidentIcon,
                            TaskDelegatedTo = participants.Where(RPT=> RPT.ActionStatus == "DELEGATED" && RPT.ActiveIncidentTaskID == ActiveIncidentTaskID)
                                               .OrderByDescending(RPT=>RPT.ActiveIncidentTaskParticipantID)                                               
                                               .Select(RPT=> new DelegatedList { FirstName = RPT.FirstName, LastName = RPT.LastName, DelegatedTo = RPT.ParticipantUserID })                                               
                                               .FirstOrDefault(),
                            TaskOwnerName = _context.Set<User>()
                                            .Where(U => U.UserId == AT.TaskOwnerId)
                                            .Select(U => new UserFullName { Firstname = U.FirstName, Lastname = U.LastName }).FirstOrDefault()
                        }).AsAsyncEnumerable();
            //task.FirstOrDefault();
            return new { task, action_participants, escalation_participants };
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    public async Task<List<IncidentTaskAudit>> GetIncidentTasksAudit(int ActiveIncidentID, int CompanyID)
    {
        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);

            var AuditData = await _context.Set<IncidentTaskAudit>().FromSqlRaw("exec Pro_Get_Incident_Tasks_Audit @ActiveIncidentID, @CompanyID",
                pActiveIncidentID, pCompanyID).ToListAsync();
            return AuditData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<Incidents> GetIncidentActivation(int ActiveIncidentID)
    {
     
        var Incident = await _context.Set<TaskHeader>().Include(x=>x.IncidentActivation)                     
                 .Where(IA=>IA.IncidentActivation.IncidentActivationId == ActiveIncidentID)
                 .Select(IA=> new Incidents()
                 {
                     Name=  IA.IncidentActivation.Name,
                     IncidentIcon= IA.IncidentActivation.IncidentIcon,
                     LaunchedOn=IA.IncidentActivation.LaunchedOn,
                     RTO = IA.Rto == null ? 0 : IA.Rto,
                     RPO = IA.Rpo == null ? 0 : IA.Rpo
                 }).FirstOrDefaultAsync();
        return Incident;
    }
    public async Task<List<FailedTaskList>> get_unattended_tasks(int CompanyID, int UserID, int ActiveIncidentID)
    {
        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pUserID = new SqlParameter("@UserID", UserID);
            var pActiveIncidentID = new SqlParameter("@ActiveIncidentID", ActiveIncidentID);

            var AuditData = await _context.Set<FailedTaskList>().FromSqlRaw("exec Pro_Get_Unattended_Tasks @CompanyID, @UserID, @ActiveIncidentID",
                pCompanyID, pUserID, pActiveIncidentID).ToListAsync();

            AuditData.Select(async c => {
                    c.TaskPredecessor = await  _active_incident_tasks_predeccessor(c.ActiveIncidentTaskID, c.IncidentActivationId);
                    return c;
                });
            return AuditData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<int> NewAdHocTask(int ActiveIncidentID,string TaskTitle,string TaskDescription, int[] ActionUsers, int[] ActionGroups, int[] EscalationUsers, int[] EscalationGroups, double EscalationDuration, double ExpectedCompletionTime, int CompanyID, int UserID, string TimeZoneId)
    {
        try
        {
            DateTime ActivationDate = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneId);
            int LastTaskSeq = 1;
            var lasttask = await  _context.Set<TaskActiveIncident>().Where(TAI=> TAI.ActiveIncidentId == ActiveIncidentID).ToListAsync();
            if (lasttask != null && lasttask.Count() > 0)
            {
                LastTaskSeq = lasttask.Max(m => m.TaskSequence) + 1;
            }

            int ActiveIncidentTaskID =await CreateActiveIncidentTask(0, ActiveIncidentID, -1, TaskTitle, TaskDescription, false,
                   EscalationDuration, ExpectedCompletionTime, LastTaskSeq, 0, (DateTime)SqlDateTime.Null, (DateTime)SqlDateTime.Null,
                    1, 0, 0, 0,0,  ActivationDate, UserID, CompanyID);

            //Create active incident participents list
            if (ActiveIncidentID > 0)
            {
               await AdHocIncidentTaskParticipants(ActiveIncidentID, ActiveIncidentTaskID, ActionUsers, ActionGroups, EscalationUsers, EscalationGroups);

                CreateTaskEscalationJob(ActiveIncidentTaskID, ActivationDate, EscalationDuration, TimeZoneId);

                List<string> grp = new List<string>();
                grp.Add("ACTION");

                string action_update = "Task " + LastTaskSeq + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(TaskTitle, 50) + "\" is now available.";
               await send_notifiation_to_groups(grp, ActiveIncidentID, ActiveIncidentTaskID, action_update, UserID, CompanyID, TimeZoneId, false, 3, sourceAction: SourceAction.NewAdHocTask);

                var actincident = await _context.Set<IncidentActivation>().Where(w => w.IncidentActivationId == ActiveIncidentID).FirstOrDefaultAsync();
                if (actincident != null)
                {
                    actincident.HasTask = true;
                    await _context.SaveChangesAsync();
                }

                return ActiveIncidentTaskID;
            }
            return 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task AdHocIncidentTaskParticipants(int ActiveIncidentID, int ActiveIncidentTaskID, int[] ActionUsers, int[] ActionGroups, int[] EscalationUsers, int[] EscalationGroups)
    {
        try
        {
            var pt_type = await  _context.Set<TaskParticipantType>().ToListAsync();
            int action_type =pt_type.Where(w => w.TaskParticipantTypeName == "ACTION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();
            int escal_type = pt_type.Where(w => w.TaskParticipantTypeName == "ESCALATION").Select(s => s.TaskParticipantTypeId).FirstOrDefault();

            var pt_list = await _context.Set<AdHocTaskParticipant>().Where(IPD=> IPD.ActiveIncidentTaskId == ActiveIncidentTaskID).ToListAsync();
            _context.RemoveRange(pt_list);
            await _context.SaveChangesAsync();

            //Create action list.
            if (ActionUsers != null)
               await adhoc_create_participant_list(ActionUsers, ActiveIncidentTaskID, action_type, "USER");
            if (ActionGroups != null)
               await adhoc_create_participant_list(ActionGroups, ActiveIncidentTaskID, action_type, "GROUP");

            //Create escalation list
            if (EscalationUsers != null)
               await adhoc_create_participant_list(EscalationUsers, ActiveIncidentTaskID, escal_type, "USER");
            if (EscalationGroups != null)
               await adhoc_create_participant_list(EscalationGroups, ActiveIncidentTaskID, escal_type, "GROUP");

            //Create Active Task Participent list

            
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "Pro_AdHoc_Create_Task_Receipient_List @IncidentActivationID, @ActiveIncidentTaskID";
                    SqlParameter parameters1 = new SqlParameter("@IncidentActivationID", ActiveIncidentID);
                    SqlParameter parameters2 = new SqlParameter("@ActiveIncidentTaskID", ActiveIncidentTaskID);

                    command.Parameters.Add(parameters1);
                    command.Parameters.Add(parameters2);

                    using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("CrisesControlDatabase")))
                    {
                        command.Connection = con;
                        if (command.Connection.State != System.Data.ConnectionState.Open)
                        {
                            command.Connection.Open();
                        }
                        var rslt = command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task adhoc_create_participant_list(int[] UList, int ActiveIncidentTaskID, int PaticipentTypeId, string objtype)
    {
        try
        {
            if (UList != null)
            {
                foreach (int item in UList)
                {
                    if (ActiveIncidentTaskID > 0 && PaticipentTypeId > 0)
                    {
                        AdHocTaskParticipant ATP = new AdHocTaskParticipant();
                        ATP.ActiveIncidentTaskId = ActiveIncidentTaskID;
                        ATP.ParticipantTypeId = PaticipentTypeId;
                        ATP.ParticipantUserId = (objtype == "USER" ? item : 0);
                        ATP.ParticipantGroupId = (objtype == "GROUP" ? item : 0);
                       await _context.AddAsync(ATP);
                       
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<bool> SaveActiveCheckListResponse(int ActiveIncidentTaskID, List<CheckListOption> CheckListResponse, int UserID, int CompanyID, string TimeZoneId)
    {
        try
        {
            var checklist = await  GetActiveTaskCheckList(ActiveIncidentTaskID, CompanyID, UserID);
            foreach (CheckListOption chkopt in CheckListResponse)
            {
                var chklistitem = checklist.Where(w => w.ActiveCheckListID == chkopt.ActiveCheckListId).FirstOrDefault();
                bool capture_response = true;

                //Check if the last response and the current response is not same then skip it.
                if (chklistitem.UserResponse != null)
                {
                    if (chkopt.MarkDone == false)
                    {
                        if (chklistitem.DoneOnly || chkopt.ActiveCheckListResponseId <= 0)
                        {
                            var usrresponse = await _context.Set<TaskActiveCheckListUserResponse>().Where(w => w.ActiveCheckListId == chkopt.ActiveCheckListId).ToListAsync();
                            _context.RemoveRange(usrresponse);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                    }

                    var last_response = chklistitem.UserResponse.OrderByDescending(o => o.ActiveReponseID).FirstOrDefault();
                    if (last_response != null)
                    {
                        if (last_response.Comment != chkopt.Response)

                            if (last_response.ActiveReponseID == chkopt.ActiveCheckListResponseId && last_response.Comment == chkopt.Response
                                && last_response.Done == chkopt.MarkDone)
                            {
                                capture_response = false;
                            }
                    }
                }

                if (capture_response)
                {
                    int UpdateActionID = 12;

                    TaskActiveCheckListUserResponse TACLUR = new TaskActiveCheckListUserResponse();
                    TACLUR.ActiveCheckListId = chkopt.ActiveCheckListId;
                    TACLUR.Comment = chkopt.Response;

                    if (chklistitem.DoneOnly)
                    {
                        TACLUR.ActiveReponseId = 0;
                        TACLUR.Done = true;
                    }
                    else
                    {
                        TACLUR.ActiveReponseId = chkopt.ActiveCheckListResponseId;
                        TACLUR.Done = chkopt.MarkDone;
                    }
                    if (chklistitem.DoneOnly || chkopt.MarkDone)
                    {
                        UpdateActionID = 13;
                    }

                    TACLUR.CreatedBy = UserID;
                    TACLUR.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await _context.AddAsync(TACLUR);
                    await _context.SaveChangesAsync();

                    var responselabel = chklistitem.CheckListOptions.Where(w => w.ActiveCheckListResponseId == chkopt.ActiveCheckListResponseId).Select(s => s.Response).FirstOrDefault();
                    string audit_text = chklistitem.Description + "<br>Response: " + responselabel + "<br/>Comment: " + chkopt.Response;

                   await AddTaskAction(ActiveIncidentTaskID, audit_text, UserID, UpdateActionID, TimeZoneId);
                    return true;
                }
              
            }
            return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


}