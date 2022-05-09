using System.Collections.Generic;

namespace CrisesControl.Core.Incidents;

public class IncidentSubset
{
    public int UserId { get; set; }
    public int[] MessageMethod { get; set; }
    public bool TrackUser { get; set; }
    public int[] UsersToNotify { get; set; }
    public bool MultiResponse { get; set; }
    public List<AckOption> AckOptions { get; set; }
    public int[] ImpactedLocationIds { get; set; }
    public List<AffectedLocation> AffectedLocations { get; set; }
    public IncidentKeyHldLst[] InitiateIncidentKeyHldLst { get; set; }
    public int AudioAssetId { get; set; } = 0;
    public int[] ImpactedLocationId { get; set; }
    public bool SilentMessage { get; set; }
    public IncidentNotificationObjLst[] InitiateIncidentNotificationObjLst { get; set; }
    public bool HasTask { get; set; }
}