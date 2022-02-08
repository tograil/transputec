using System;

namespace CrisesControl.Core.Models
{
    public partial class InvoiceStatus
    {
        public int InvoiceStatusId { get; set; }
        public string Name { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
