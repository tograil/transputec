using System;

namespace CrisesControl.Core.Models
{
    public partial class TourLog
    {
        public int Id { get; set; }
        public string? TourName { get; set; }
        public int? TourStepId { get; set; }
        public int? UserId { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
    }
}
