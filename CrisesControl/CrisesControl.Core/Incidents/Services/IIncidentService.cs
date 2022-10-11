using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents.Services;

public interface IIncidentService
{
    void InitiateIncident(IncidentActivation activation, IncidentSubset incidentSubset, CancellationToken cancellationToken = default);
    void InitiateAndLaunchIncident(IncidentActivation activation, IncidentSubset incidentSubset, CancellationToken cancellationToken = default);
}