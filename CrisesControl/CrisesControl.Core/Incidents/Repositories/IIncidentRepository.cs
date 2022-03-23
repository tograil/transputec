using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Models;

namespace CrisesControl.Core.Incidents.Repositories;

public interface IIncidentRepository
{
    Task<bool> CheckDuplicate(int companyId, string incidentName, int incidentId);

    Task<Incident?> GetIncident(int companyId, int incidentId);

    Task<int> AddIncident(Incident incident);

    Task AddIncidentActivation(IncidentActivation incidentActivation, CancellationToken cancellationToken);

    Task<IncidentActivation?> GetIncidentActivation(int companyId, int incidentActivationId);

    Task AddIncidentKeyContacts(ICollection<IncidentKeyContact> contacts);

    Task ProcessKeyHolders(int companyId, int incidentId, int currentUserId, int[] keyHolders);

    Task SaveIncidentMessageResponse(ICollection<AckOption> ackOptions, int incidentId);

    Task AddIncidentGroup(int incidentId, int[] groups, int companyId);

    Task CreateIncidentSegLinks(int incidentId, int userId, int companyId);

    NewIncident CloneIncident(int incidentId, bool keepKeyContact, bool keepIncidentMessage, bool keepTasks,
        bool keepIncidentAsset, bool keepTaskAssets,
        bool keepTaskCheckList, bool keepIncidentParticipants, int status, int currentUserId, int companyId,
        string timeZoneId);

    Task<ICollection<DataIncidentType>> CopyIncidentTypes(int userId, int companyId);

    Task CopyIncidentToCompany(int companyId, int userId, string timeZoneId = "GMT Standard Time");

    Task<string> GetStatusName(int status);
}