using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using IncidentActivation = CrisesControl.Core.Incidents.IncidentActivation;
using Location = CrisesControl.Core.LocationAggregate.Location;

namespace CrisesControl.Infrastructure.Repositories;

public class ActiveIncidentRepository : IActiveIncidentRepository
{
    private readonly CrisesControlContext _context;

    public ActiveIncidentRepository(CrisesControlContext context)
    {
        _context = context;
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
}