using System;

namespace CrisesControl.Core.Models
{
    public partial class CustomEventLogHeader
    {
        public long EventLogHeaderId { get; set; }
        public int ActiveIncidentId { get; set; }
        public int? PermittedDepartment { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
