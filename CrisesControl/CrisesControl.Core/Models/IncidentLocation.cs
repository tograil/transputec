namespace CrisesControl.Core.Models
{
    public partial class IncidentLocation
    {
        public int LocationId { get; set; }
        public int? CompanyId { get; set; }
        public int? LibLocationId { get; set; }
        public int? IncidentActivationId { get; set; }
        public int? ImpactedLocationId { get; set; }
        public bool Display { get; set; }
    }
}
