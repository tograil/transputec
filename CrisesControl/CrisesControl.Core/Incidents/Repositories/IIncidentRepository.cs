namespace CrisesControl.Core.Incidents.Repositories;

public interface IIncidentRepository
{
    bool CheckDuplicate(int companyId, string incidentName, int incidentId);
}