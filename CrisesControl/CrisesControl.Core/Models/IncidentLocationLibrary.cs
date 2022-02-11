namespace CrisesControl.Core.Models
{
    public partial class IncidentLocationLibrary
    {
        public int LocationId { get; set; }
        public int? CompanyId { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public string? LocationType { get; set; }
    }
}
