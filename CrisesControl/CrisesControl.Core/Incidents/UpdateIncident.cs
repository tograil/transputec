using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class UpdateIncident
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        //public int CompanyId { get; set; }
        public int IncidentActivationId { get; set; }
        public int? Severity { get; set; }
        //public int SafetyFlag { get; set; }
        public int? ImpactedLocationId { get; set; }
        public string? ImpactedLocation { get; set; }
        public int? IncidentId { get; set; }
        public DateTimeOffset InitiatedOn { get; set; }
        public int? InitiatedBy { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public int? LaunchedBy { get; set; }
        public DateTimeOffset DeactivatedOn { get; set; }
        public int? DeactivatedBy { get; set; }
        public DateTimeOffset ClosedOn { get; set; }
        public int? ClosedBy { get; set; }
        public int? NumberOfKeyHolders { get; set; }
        public int? StatusId { get; set; }
        public bool HasNotes { get; set; }
        public bool HasTask { get; set; }
        public bool TrackUser { get; set; }
        public string? Status { get; set; }
    }
}
