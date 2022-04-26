using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;

namespace CrisesControl.Core.Incidents
{
    public partial class UpdateIncidentStatusReturn
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public int IncidentActivationId { get; set; }
        public int IncidentId { get; set; }
        public DateTimeOffset InitiatedOn { get; set; }
        public int InitiatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserFullName InitiatedByName { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public int LaunchedBy { get; set; }
        public UserFullName LaunchedByName { get; set; }
        public DateTimeOffset DeactivatedOn { get; set; }
        public int DeactivatedBy { get; set; }
        public UserFullName DeactivatedByName { get; set; }
        public DateTimeOffset ClosedOn { get; set; }
        public int ClosedBy { get; set; }
        public UserFullName ClosedByName { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public bool HasTask { get; set; }
        public int Severity { get; set; }
        public int SafetyFlag { get; set; }
        public int ImpactedLocationId { get; set; }
        public string ImpactedLocation { get; set; }
        public int NumberOfKeyHolders { get; set; }
        public int AssetId { get; set; }
        public int PlanAssetID { get; set; }
        public bool HasNotes { get; set; }
        public bool TrackUser { get; set; }
        public bool SilentMessage { get; set; }
        public List<CommsMethods> MessageMethod { get; set; }
        public List<IIncKeyConResponse> IncKeyCon { get; set; }
        public List<IIncNotificationLst> IncNotificationLst { get; set; }
        public List<ActionLsts> ActionLst { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public List<AffectedLocation> AffectedLocations { get; set; }
        public int TotalAffectedLocations { get; set; }
        public bool ShowTrackUser { get; set; }
        public bool ShowSilentMessage { get; set; }
        public bool ShowMessageMethod { get; set; }
        public bool IsKeyContact { get; set; }
        public int TotalSOSEvents { get; set; }
        public bool IsSOS { get; set; }
        public int SegregationWarning { get; set; }
        public List<IIncKeyConResponse> UsersToNotify { get; set; }
        public List<IncidentParticipants> Participants { get; set; }
        public string ErrorCode { get; set; }
        public string SocialHandle { get; set; }
        public List<SocialHandles> SocialHandles { get; set; }
        public int CascadePlanID { get; set; }
        public List<IIncKeyConResponse> IncKeyholders { get; set; }
    }
}
