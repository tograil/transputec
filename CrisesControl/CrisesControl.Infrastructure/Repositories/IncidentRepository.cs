using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

    public async Task<int> AddIncident(Incident incident)
    {
        await _context.AddAsync(incident);

        await _context.SaveChangesAsync();

        return incident.IncidentId;
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
}