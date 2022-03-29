using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.LaunchCompanyIncident;

public class LaunchCompanyIncidentRequest : IRequest<LaunchCompanyIncidentResponse>
{
    public int IncidentActivationtId { get; set; }
    public string Description { get; set; }
    public int Severity { get; set; }
    public bool MultiResponse { get; set; }
    public List<AckOption> AckOptions { get; set; }
    public int[] ImpactedLocationId { get; set; }
    public string AudioMessage { get; set; }
    public double AssetSize { get; set; }
    public int AudioAssetId { get; set; }
    public bool TrackUser { get; set; }
    public bool SilentMessage { get; set; }
    public int[] MessageMethod { get; set; }
    public LaunchIncidentActionLst[] LaunchIncidentActionLst { get; set; }
    public IncidentKeyHldLst[] LaunchIncidentKeyHldLst { get; set; }
    public IncidentNotificationObjLst[] LaunchIncidentNotificationObjLst { get; set; }
    public List<AffectedLocation> AffectedLocations { get; set; }
    public int[] UsersToNotify { get; set; }
    public List<string> SocialHandle { get; set; }
    public int CascadePlanID { get; set; }
}