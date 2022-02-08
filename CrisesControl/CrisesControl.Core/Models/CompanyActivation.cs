using System;

namespace CrisesControl.Core.Models
{
    public partial class CompanyActivation
    {
        public int ActivationId { get; set; }
        public int CompanyId { get; set; }
        public string? ActivationKey { get; set; }
        public int Status { get; set; }
        public string? Ipaddress { get; set; }
        public int SalesSource { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int ActivatedBy { get; set; }
        public DateTimeOffset ActivatedOn { get; set; }
    }
}
