using System;

namespace CrisesControl.Core.Models
{
    public partial class IndustrySector
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
