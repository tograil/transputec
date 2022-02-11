namespace CrisesControl.Core.Models
{
    public partial class SegIncidentLocationLink
    {
        public int IncidentLocationId { get; set; }
        public int LocationId { get; set; }
        public int IncidentId { get; set; }
        public int CompanyId { get; set; }
    }
}
