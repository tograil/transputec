using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.InitiateAndLaunchIncident;

public class InitiateAndLaunchIncidentRequest : IRequest<InitiateAndLaunchIncidentResponse>
{
    public int IncidentId { get; set; }
    public int IncidentActivationId { get; set; }
    public string Description { get; set; }
    public int Severity { get; set; }
    public bool MultiResponse { get; set; }
    public List<AckOption> AckOptions { get; set; }
    public int[] ImpactedLocationId { get; set; }
    public string UserRole { get; set; }
    public int AudioAssetId { get; set; } = 0;
    public bool TrackUser { get; set; }
    public bool SilentMessage { get; set; }
    public int[] MessageMethod { get; set; }
    public int NumberOfKeyHolder { get; set; }
    public IncidentActionLst[] InitiateIncidentActionLst { get; set; }
    public IncidentKeyHldLst[] InitiateIncidentKeyHldLst { get; set; }
    public IncidentNotificationObjLst[] InitiateIncidentNotificationObjLst { get; set; }
    public List<AffectedLocation> AffectedLocations { get; set; }
    public int[] UsersToNotify { get; set; }
    public int LaunchMode { get; set; }
    public List<string> SocialHandle { get; set; }
    public string Source { get; set; }
    public int CascadePlanId { get; set; } = 0;
    public int[] IncidentKeyholder { get; set; }
}