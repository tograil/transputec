using System;

namespace CrisesControl.Core.Models
{
    public partial class Sosaction
    {
        public int SosactionId { get; set; }
        public int SosalertId { get; set; }
        public int CompanyId { get; set; }
        public int MessageId { get; set; }
        public string? ActionType { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
