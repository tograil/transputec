using System;

namespace CrisesControl.Core.Models
{
    public partial class IncidentSop
    {
        public int IncidentSopid { get; set; }
        public int SopheaderId { get; set; }
        public int IncidentId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int AssetId { get; set; }
    }
}
