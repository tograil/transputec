using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents.Repositories;

public interface IActiveIncidentRepository
{
    Task ProcessKeyHolders(int companyId, int incidentId, int activeIncidentId, int currentUserId,
        int[] incidentKeyHolders);

    Task ProcessImpactedLocation(int[] locationIds, int incidentActivationId, int companyId, string action);

    Task ProcessAffectedLocation(ICollection<AffectedLocation> affectedLocations, int incidentActivationId,
        int companyId, string type = "AFFECTED", string action = "INITIATE");

    Task<ICollection<IncidentActivation>> GetIncidentActivationList(int incidentActivationId, int companyId);

    Task CreateActiveKeyContact(int incidentActivationId, int incidentId,
        IncidentKeyHldLst[] keyHldLst, int currentUserId, int companyId, string timeZoneId);

    Task<int> CreateActiveIncidentTask(int activeIncidentTaskId, int activeIncidentId, int incidentTaskId,
        string taskTitle,
        string taskDescription, bool hasPredecessor, double escalationDuration, double expectedCompletionTime,
        int taskSequence,
        int taskOwnerId, DateTime taskAcceptedDate, DateTime taskEscalatedDate, int taskStatus, int taskCompletedBy,
        int nextIncidentTaskId,
        int previousIncidentTaskId, int previousOwnerId, DateTimeOffset taskActivationDate, int currentUserId,
        int companyId);

    Task CreateActiveCheckList(int activeIncidentTaskId, int incidentTaskId, int userId, string timeZoneId = "GMT Standard Time");

    Task CreateTaskRecipient(int activeIncidentId, int activeIncidentTaskId, int incidentTaskId);
}