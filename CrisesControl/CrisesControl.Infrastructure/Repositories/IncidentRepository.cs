using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using IncidentActivation = CrisesControl.Core.Incidents.IncidentActivation;

namespace CrisesControl.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly CrisesControlContext _context;
    public IncidentRepository(CrisesControlContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckDuplicate(int companyId, string incidentName, int incidentId)
    {
        if (incidentId == 0)
            return !await _context.Set<Incident>().AnyAsync(x => x.CompanyId == companyId 
                                                                    && x.Name == incidentName && x.Status != 3);

        return await _context.Set<Incident>().AnyAsync(x => x.CompanyId == companyId
                                                 && x.Name == incidentName && x.Status != 3 && x.IncidentId == incidentId);
    }

    public async Task<Incident?> GetIncident(int companyId, int incidentId)
    {
        return await _context.Set<Incident>()
            .FirstOrDefaultAsync(x => x.IncidentId == incidentId && x.CompanyId == companyId);
    }

    public async Task<int> AddIncident(Incident incident)
    {
        await _context.AddAsync(incident);

        await _context.SaveChangesAsync();

        return incident.IncidentId;
    }

    public async Task AddIncidentActivation(IncidentActivation incidentActivation, CancellationToken cancellationToken)
    {
        await _context.AddAsync(incidentActivation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateIncidentActivation(IncidentActivation incidentActivation, CancellationToken cancellationToken)
    {
        _context.Update(incidentActivation);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IncidentActivation?> GetIncidentActivation(int companyId, int incidentActivationId)
    {
        var result = await _context.Set<IncidentActivation>()
            .Include(x => x.Incident)
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.IncidentActivationId == incidentActivationId);

        return result;
    }

    public async Task AddIncidentKeyContacts(ICollection<IncidentKeyContact> contacts)
    {
        await _context.AddRangeAsync(contacts);

        await _context.SaveChangesAsync();
    }

    public async Task ProcessKeyHolders(int companyId, int incidentId, int currentUserId, int[] keyHolders)
    {
        var incKeyHolders = _context.Set<IncidentKeyholder>()
            .Where(x => x.CompanyID == companyId
                        && x.IncidentID == incidentId
                        && (x.ActiveIncidentID == null ||
                            x.ActiveIncidentID == 0))
            .ToArray();

        var keyList = new List<long>();

        foreach (var keyHolderId in keyHolders)
        {
            if (keyHolderId <= 0) continue;

            var keyHld = incKeyHolders
                .FirstOrDefault(s => s.CompanyID == companyId
                                     && s.IncidentID == incidentId && s.UserID == keyHolderId);
            if (keyHld is not null)
            {
                keyList.Add(keyHld.IncidentKeyholderID);
            }
            else
            {
                var incNewIncKeyHolder = new IncidentKeyholder
                {
                    CompanyID = companyId,
                    IncidentID = incidentId,
                    UserID = keyHolderId
                };

                await _context.AddAsync(incNewIncKeyHolder);
            }
        }

        foreach (var incidentKeyholder in incKeyHolders)
        {
            var isDel = keyList.Any(s => s == incidentKeyholder.IncidentKeyholderID);
            if (!isDel)
            {
                _context.Set<IncidentKeyholder>().Remove(incidentKeyholder);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task SaveIncidentMessageResponse(ICollection<AckOption> ackOptions, int incidentId)
    {
        var prevOptions = await _context.Set<IncidentMessageResponse>()
            .Where(im => im.IncidentId == incidentId)
            .ToListAsync();

        _context.Set<IncidentMessageResponse>().RemoveRange(prevOptions);
        await _context.SaveChangesAsync();

        var incidentMessageToAdd = ackOptions.Select(x =>
            new IncidentMessageResponse
            {
                ResponseId = x.ResponseId,
                ResponseCode = x.ResponseCode,
                IncidentId = incidentId
            })
            .ToList();

        await _context.AddRangeAsync(incidentMessageToAdd);

        await _context.SaveChangesAsync();
    }

    public async Task AddIncidentGroup(int incidentId, int[] groups, int companyId)
    {
        var linksToDel = await _context.Set<SegGroupIncidentLink>()
            .Where(x => x.IncidentId == incidentId)
            .ToListAsync();

        _context.Set<SegGroupIncidentLink>().RemoveRange(linksToDel);

        await _context.SaveChangesAsync();

        var groupsToSave = groups.Select(x => new SegGroupIncidentLink
        {
            CompanyId = companyId,
            GroupId = x,
            IncidentId = incidentId
        }).ToList();

        await _context.AddRangeAsync(groupsToSave);

        await _context.SaveChangesAsync();
    }

    public async Task CreateIncidentSegLinks(int incidentId, int userId, int companyId)
    {
        var pCompanyId = new SqlParameter("@CompanyId", companyId);
        var pUserId = new SqlParameter("@UserId", userId);
        var pIncidentId = new SqlParameter("@IncidentId", incidentId);

        await _context.Database.ExecuteSqlRawAsync("exec Pro_Incident_CreateLink @CompanyId, @UserId, @IncidentId", pCompanyId, pUserId, pIncidentId);
    }

    public NewIncident CloneIncident(int incidentId, bool keepKeyContact, bool keepIncidentMessage, bool keepTasks,
        bool keepIncidentAsset, bool keepTaskAssets, bool keepTaskCheckList, bool keepIncidentParticipants, int status,
        int currentUserId, int companyId, string timeZoneId)
    {
        var pIncidentId = new SqlParameter("@IncidentID", incidentId);
        var pKeepKeyContact = new SqlParameter("@KeepKeyContact", keepKeyContact);
        var pKeepIncidentMessage = new SqlParameter("@KeepIncidentMessage", keepIncidentMessage);
        var pKeepTasks = new SqlParameter("@KeepTasks", keepTasks);
        var pKeepIncidentAsset = new SqlParameter("@KeepIncidentAsset",keepIncidentAsset);
        var pKeepTaskAssets = new SqlParameter("@KeepTaskAssets", keepTaskAssets);
        var pKeepTaskCheckList = new SqlParameter("@KeepTaskCheckList", keepTaskCheckList);
        var pKeepIncidentParticipants = new SqlParameter("@KeepIncidentParticipants", keepIncidentParticipants);
        var pStatus = new SqlParameter("@Status", status);
        var pCurrentUserId = new SqlParameter("@CurrentUserID", currentUserId);
        var pCompanyId = new SqlParameter("@CompanyID", companyId);
        var pCustomerTime = new SqlParameter("@CustomerTime", DateTime.Now.GetDateTimeOffset(timeZoneId));

        var result = _context.Set<NewIncident>().FromSqlRaw("exec Pro_Copy_Incident {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
            pIncidentId, pKeepKeyContact, pKeepIncidentMessage, pKeepTasks, pKeepIncidentAsset, pKeepTaskAssets, pKeepTaskCheckList, pKeepIncidentParticipants, pStatus, pCurrentUserId, pCompanyId, pCustomerTime).ToList().First();

        return result;
    }

    public async Task<ICollection<DataIncidentType>> CopyIncidentTypes(int userId, int companyId)
    {
        var libIncidentTypeData = await _context.Set<LibIncidentType>().ToListAsync();

        var dataIncidentTypes = new List<DataIncidentType>();

        foreach (var libIncidentType in libIncidentTypeData)
        {
            var incType = await _context.Set<IncidentType>()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && libIncidentType.Name == x.Name);

            if (incType is not null)
            {
                dataIncidentTypes.Add(new DataIncidentType
                {
                    IncidentTypeId = incType.IncidentTypeId,
                    Name = incType.Name!,
                    LibIncidentType = libIncidentType.LibIncidentTypeId
                });
            }
            else
            {
                var newIncidentType = new IncidentType()
                {
                    CompanyId = companyId,
                    Name = libIncidentType.Name,
                    Status = 1
                };

                await _context.AddAsync(newIncidentType);
                await _context.SaveChangesAsync();

                dataIncidentTypes.Add(new DataIncidentType
                {
                    IncidentTypeId = newIncidentType.IncidentTypeId,
                    Name = newIncidentType.Name,
                    LibIncidentType = libIncidentType.LibIncidentTypeId
                });
            }
        }

        return dataIncidentTypes;
    }

    public async Task CopyIncidentToCompany(int companyId, int userId, string timeZoneId = "GMT Standard Time")
    {
        var sourceIncidentTypes = await CopyIncidentTypes(userId, companyId);

        var incidentsFromLib =
            await _context.Set<LibIncident>().Where(x => x.IsDefault == 1 && x.Status != 3).ToListAsync();

        foreach (var incident in incidentsFromLib)
        {
            var isIncidentExists = await _context.Set<Incident>()
                .AnyAsync(x => x.CompanyId == companyId && x.Name == incident.Name);

            if (!isIncidentExists)
            {
                var incidentType =
                    sourceIncidentTypes.FirstOrDefault(x => x.LibIncidentType == incident.LibIncidentTypeId);

                if (incidentType != null)
                {
                    var keyHolderList = _context.Set<User>().Where(x => x.CompanyId == companyId && x.Status != 3)
                        .Select(x => new AddIncidentKeyHldLst(x.UserId));

                    var ackOptions = new List<AckOption>();

                    var incidentNew = new Incident
                    {
                        CompanyId = companyId,
                        IncidentIcon = incident.LibIncodentIcon,
                        Name = incident.Name,
                        Description = incident.Description,
                        PlanAssetId = 0,
                        IncidentTypeId = incidentType.IncidentTypeId,
                        Severity = incident.Severity,
                        Status = incident.Status,
                        NumberOfKeyHolders = 1,
                        AudioAssetId = 0,
                        TrackUser = false,
                        SilentMessage = false,
                        CreatedBy = userId,
                        CreatedOn = DateTimeOffset.UtcNow,
                        UpdatedBy = userId,
                        IsSos = false,
                        CascadePlanId = 0,
                        UpdatedOn = DateTimeOffset.UtcNow
                    };

                    var incidentId = await AddIncident(incidentNew);

                    var contacts = keyHolderList
                        .Select(x => new IncidentKeyContact
                        {
                            CompanyId = companyId,
                            IncidentId = incidentId,
                            UserId = x.UserId ?? 0,
                            CreatedBy = userId,
                            CreatedOn = DateTimeOffset.UtcNow,
                            UpdatedBy = userId,
                            UpdatedOn = DateTimeOffset.UtcNow
                        }).ToArray();

                    await AddIncidentKeyContacts(contacts);

                    await ProcessKeyHolders(companyId, incidentId, userId, Array.Empty<int>());

                    await SaveIncidentMessageResponse(ackOptions, incidentId);

                    await AddIncidentGroup(incidentId, Array.Empty<int>(), companyId);

                    await CreateIncidentSegLinks(incidentId, userId, companyId);
                }
            }
        }
    }

    public async Task<string> GetStatusName(int status)
    {
        return (await _context.Set<SysParameter>()
            .FirstAsync(x => x.Value == status.ToString()
                             && x.Category == "IncidentStatus")).Name;
    }
}