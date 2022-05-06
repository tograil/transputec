using System.Collections.Generic;

namespace CrisesControl.Core.Incidents;

public class IncidentSubset
{
    public int[] MessageMethod { get; set; }
    public bool TrackUser { get; set; }
    public int[] UsersToNotify { get; set; }
    public bool MultiResponse { get; set; }
    public List<AckOption> AckOptions { get; set; }
    public int[] ImpactedLocationIds { get; set; }
    public List<AffectedLocation> AffectedLocations { get; set; }
}