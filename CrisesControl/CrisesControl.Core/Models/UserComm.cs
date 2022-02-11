using System;

namespace CrisesControl.Core.Models
{
    public partial class UserComm
    {
        public int UserCommsId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public int MethodId { get; set; }
        public string MessageType { get; set; } = null!;
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int Priority { get; set; }
    }
}
