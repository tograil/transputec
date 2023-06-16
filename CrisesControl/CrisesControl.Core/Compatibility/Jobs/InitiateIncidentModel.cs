using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CrisesControl.Core.Incidents;

namespace CrisesControl.Core.Compatibility.Jobs;

[Obsolete("Added for compatibility with old portal")]
public class InitiateIncidentModel : CcBase
{
    public InitiateIncidentModel()
    {
        AudioAssetId = 0;
        CascadePlanID = 0;
    }

    [Required(ErrorMessage = "Incident ID can not be blank")]
    public int IncidentId { get; set; }
    public int IncidentActivationId { get; set; }
    public string Description { get; set; }
    //[Required(ErrorMessage = "Severity is not provided")]
    public int Severity { get; set; }
    public bool MultiResponse { get; set; }
    public List<AckOption> AckOptions { get; set; }
    //[Required(ErrorMessage = "Location id is not provided")]
    public int[] ImpactedLocationId { get; set; }
    public string UserRole { get; set; }
    public int AudioAssetId { get; set; }
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
    public int CascadePlanID { get; set; }
    public int[] IncidentKeyholder { get; set; }
}