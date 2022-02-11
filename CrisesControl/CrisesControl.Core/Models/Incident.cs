using System;

namespace CrisesControl.Core.Models
{
    public partial class Incident
    {
        public int IncidentId { get; set; }
        public int CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int IncidentTypeId { get; set; }
        public string? IncidentIcon { get; set; }
        public string? IncidentPlanDoc { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int Severity { get; set; }
        public int NumberOfKeyHolders { get; set; }
        public int AudioAssetId { get; set; }
        public bool IsSopdoc { get; set; }
        public bool HasTask { get; set; }
        public int SopdocId { get; set; }
        public bool TrackUser { get; set; }
        public bool SilentMessage { get; set; }
        public bool IsSos { get; set; }
        public int PlanAssetId { get; set; }
        public int CascadePlanId { get; set; }
    }
}
