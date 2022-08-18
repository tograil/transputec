using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Incidents;

public class IncidentDetails
{
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int IncidentId { get; set; }
    public string IncidentTypeName { get; set; }
    public int IncidentTypeId { get; set; }
    public int Severity { get; set; }
    public string IncidentIcon { get; set; }
    public int PlanAssetID { get; set; }
    public string ActionPlanName { get; set; }
    public int Status { get; set; }
    public int NumberOfKeyHolders { get; set; }
    //public int ErrorId { get; set; }
    [NotMapped]
    public List<IncKeyCons> IncKeyCon { get; set; }
    [NotMapped]
    public List<IncKeyCons> IncKeyholders { get; set; }

    public int MessageCount { get; set; }
    [NotMapped]
    public IEnumerable<ActionLsts> ActionLst { get; set; }
    [NotMapped]
    public IEnumerable<IncidentAssets> IncidentAssets { get; set; }
    public int AssetId { get; set; }
    public bool HasTask { get; set; }
    public bool IsSOPDoc { get; set; }
    public bool TrackUser { get; set; }
    public bool SilentMessage { get; set; }
    [NotMapped]
    public List<AckOption> AckOptions { get; set; }
    [NotMapped]
    public List<CommsMethods> MessageMethods { get; set; }
    public bool ShowTrackUser { get; set; }
    public bool ShowSilentMessage { get; set; }
    public bool ShowMessageMethod { get; set; }
    [NotMapped]
    public List<ImpactedLocation> ImpactedLocation { get; set; }
    [NotMapped]
    public List<IIncNotificationLst> NotificationGroups { get; set; }
    public int SegregationWarning { get; set; }
    [NotMapped]
    public UserFullName CreatedByName { get; set; }
    [NotMapped]
    public UserFullName UpdatedByName { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    [NotMapped]
    public List<IncidentParticipants> Participants { get; set; }
    public int CascadePlanID { get; set; }
    [NotMapped]
    public bool HasNominatedKeyholders { get; set; }
}