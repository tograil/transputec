using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents.Services;

public interface IIncidentService
{
    Task InitiateIncident(IncidentActivation activation, IncidentSubset incidentSubset, CancellationToken cancellationToken = default);
    Task InitiateAndLaunchIncident(IncidentActivation activation, CancellationToken cancellationToken);
}