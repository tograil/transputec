using System;

namespace CrisesControl.Core.Models
{
    public partial class LibIncidentAction
    {
        public int LibIncidentActionId { get; set; }
        public int LibIncidentId { get; set; }
        public int SequenceNo { get; set; }
        public string? ActionDescription { get; set; }
        public bool ResponseRequired { get; set; }
        public int Predecessor { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
