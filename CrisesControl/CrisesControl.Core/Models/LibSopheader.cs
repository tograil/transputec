using System;

namespace CrisesControl.Core.Models
{
    public partial class LibSopheader
    {
        public int LibSopheaderId { get; set; }
        public int CompanyId { get; set; }
        public string? Sopversion { get; set; }
        public int IncidentId { get; set; }
        public DateTimeOffset ReviewDate { get; set; }
        public int NoOfUse { get; set; }
        public int TotalVotes { get; set; }
        public int TotalRating { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
