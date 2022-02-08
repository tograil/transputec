using System;

namespace CrisesControl.Core.Models
{
    public partial class LibIncident
    {
        public int LibIncidentId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int LibIncidentTypeId { get; set; }
        public string? LibIncodentIcon { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int Severity { get; set; }
        public int IsDefault { get; set; }
        public bool IsSos { get; set; }
    }
}
