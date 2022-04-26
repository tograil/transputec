using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;

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
    public List<IncKeyCons> IncKeyCon { get; set; }
    public List<IncKeyCons> IncKeyholders { get; set; }

    public int MessageCount { get; set; }
    public IEnumerable<ActionLsts> ActionLst { get; set; }
    public IEnumerable<IncidentAssets> IncidentAssets { get; set; }
    public int AssetId { get; set; }
    public bool HasTask { get; set; }
    public bool IsSOPDoc { get; set; }
    public bool TrackUser { get; set; }
    public bool SilentMessage { get; set; }
    public List<AckOption> AckOptions { get; set; }
    public List<CommsMethods> MessageMethods { get; set; }
    public bool ShowTrackUser { get; set; }
    public bool ShowSilentMessage { get; set; }
    public bool ShowMessageMethod { get; set; }
    public List<int?> ImpactedLocation { get; set; }
    public List<IIncNotificationLst> NotificationGroups { get; set; }
    public int SegregationWarning { get; set; }
    public UserFullName CreatedByName { get; set; }
    public UserFullName UpdatedByName { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public List<IncidentParticipants> Participants { get; set; }
    public int CascadePlanID { get; set; }
    public bool HasNominatedKeyholders { get; set; }
}