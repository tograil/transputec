using System;

namespace CrisesControl.Core.Models
{
    public partial class CompanyComm
    {
        public int CompanyCommsId { get; set; }
        public int CompanyId { get; set; }
        public int MethodId { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public bool ServiceStatus { get; set; }
    }
}
