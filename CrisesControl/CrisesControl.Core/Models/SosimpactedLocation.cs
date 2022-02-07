namespace CrisesControl.Core.Models
{
    public partial class SosimpactedLocation
    {
        public int LocationId { get; set; }
        public int? IncidentId { get; set; }
        public int? ImpactedLocationId { get; set; }
    }
}
