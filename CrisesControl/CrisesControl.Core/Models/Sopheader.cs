using System;

namespace CrisesControl.Core.Models
{
    public partial class Sopheader
    {
        public int SopheaderId { get; set; }
        public int CompanyId { get; set; }
        public string? Sopversion { get; set; }
        public DateTimeOffset ReviewDate { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int ReminderCount { get; set; }
        public string? ReviewFrequency { get; set; }
        public int Sopowner { get; set; }
    }
}
