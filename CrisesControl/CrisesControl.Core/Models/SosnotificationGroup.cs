namespace CrisesControl.Core.Models
{
    public partial class SosnotificationGroup
    {
        public int SosnotificationId { get; set; }
        public int IncidentId { get; set; }
        public int CompanyId { get; set; }
        public int ObjectMappingId { get; set; }
        public int SourceObjectPrimaryId { get; set; }
    }
}
