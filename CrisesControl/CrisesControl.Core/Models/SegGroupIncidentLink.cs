namespace CrisesControl.Core.Models
{
    public partial class SegGroupIncidentLink
    {
        public int GroupIncidentId { get; set; }
        public int IncidentId { get; set; }
        public int GroupId { get; set; }
        public int CompanyId { get; set; }
    }
}
