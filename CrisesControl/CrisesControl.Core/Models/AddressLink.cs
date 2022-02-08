using System;

namespace CrisesControl.Core.Models
{
    public partial class AddressLink
    {
        public int AddressLinkId { get; set; }
        public int AddressId { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
