using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents.Services;

public interface IActiveIncidentTaskService
{
    Task StartTaskAllocation(int incidentId, int activeIncidentId, int currentUserId, int companyId);

    Task CopyAssets(int incidentId, int activeIncidentId, int messageId);
}