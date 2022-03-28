using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents.Repositories;

public interface IActiveIncidentRepository
{
    Task ProcessImpactedLocation(int[] locationIds, int incidentActivationId, int companyId, string action);

    Task ProcessAffectedLocation(ICollection<AffectedLocation> affectedLocations, int incidentActivationId,
        int companyId, string type = "AFFECTED", string action = "INITIATE");

    Task CreateActiveKeyContact(int incidentActivationId, int incidentId,
        IncidentKeyHldLst[] keyHldLst, int currentUserId, int companyId, string timeZoneId);
}