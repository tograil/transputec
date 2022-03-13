using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.Core.Models;

namespace CrisesControl.Core.Incidents.Repositories;

public interface IIncidentRepository
{
    Task<bool> CheckDuplicate(int companyId, string incidentName, int incidentId);

    Task<int> AddIncident(Incident incident);

    Task AddIncidentKeyContacts(ICollection<IncidentKeyContact> contacts);

    Task ProcessKeyHolders(int companyId, int incidentId, int currentUserId, int[] keyHolders);

    Task SaveIncidentMessageResponse(ICollection<AckOption> ackOptions, int incidentId);

    Task AddIncidentGroup(int incidentId, int[] groups, int companyId);

    Task CreateIncidentSegLinks(int incidentId, int userId, int companyId);
}