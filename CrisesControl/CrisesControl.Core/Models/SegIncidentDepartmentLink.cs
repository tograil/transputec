namespace CrisesControl.Core.Models
{
    public partial class SegIncidentDepartmentLink
    {
        public int IncidentDepartmentId { get; set; }
        public int DepartmentId { get; set; }
        public int IncidentId { get; set; }
        public int CompanyId { get; set; }
    }
}
