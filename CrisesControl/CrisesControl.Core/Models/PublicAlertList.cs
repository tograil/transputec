using System;

namespace CrisesControl.Core.Models
{
    public partial class PublicAlertList
    {
        public int ListId { get; set; }
        public string ListName { get; set; } = null!;
        public int TotalUsers { get; set; }
        public string? FileName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
