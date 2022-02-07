using System;

namespace CrisesControl.Core.Models
{
    public partial class ActiveIncidentKeyContact
    {
        public int ActiveIncidentKeyContactId { get; set; }
        public int? IncidentId { get; set; }
        public int? IncidentActionId { get; set; }
        public int UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int IncidentActivationId { get; set; }
    }
}
