using System;

namespace CrisesControl.Core.Models
{
    public partial class UserLoginLog
    {
        public int UserLoginLogId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset LoggedInTime { get; set; }
        public string Ipaddress { get; set; } = null!;
        public string? DeviceType { get; set; }
    }
}
