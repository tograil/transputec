namespace CrisesControl.Core.Models
{
    public partial class IncidentType
    {
        public int IncidentTypeId { get; set; }
        public int CompanyId { get; set; }
        public string? Name { get; set; }
        public int Status { get; set; }
    }
}
