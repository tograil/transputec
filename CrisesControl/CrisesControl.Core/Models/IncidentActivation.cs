using System;

namespace CrisesControl.Core.Models
{
    public partial class IncidentActivation
    {
        public int IncidentActivationId { get; set; }
        public int IncidentId { get; set; }
        public string? IncidentDescription { get; set; }
        public DateTimeOffset InitiatedOn { get; set; }
        public int? InitiatedBy { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public int? LaunchedBy { get; set; }
        public DateTimeOffset DeactivatedOn { get; set; }
        public int? DeactivatedBy { get; set; }
        public DateTimeOffset ClosedOn { get; set; }
        public int? ClosedBy { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CompanyId { get; set; }
        public int Severity { get; set; }
        public int ImpactedLocationId { get; set; }
        public int AssetId { get; set; }
        public bool HasNotes { get; set; }
        public bool TrackUser { get; set; }
        public bool SilentMessage { get; set; }
        public int PlanAssetId { get; set; }
        public int LaunchMode { get; set; }
        public string? Name { get; set; }
        public string? IncidentIcon { get; set; }
        public bool HasTask { get; set; }
        public string? SocialHandle { get; set; }
        public int CascadePlanId { get; set; }
        
    }
}
