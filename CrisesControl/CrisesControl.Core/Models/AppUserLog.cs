using System;

namespace CrisesControl.Core.Models
{
    public partial class AppUserLog
    {
        public int LogId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset LogDateTime { get; set; }
        public string DeviceId { get; set; } = null!;
        public string Lat { get; set; } = null!;
        public string Lng { get; set; } = null!;
        public string MethodName { get; set; } = null!;
        public string ControllerName { get; set; } = null!;
        public DateTimeOffset RequestDateTime { get; set; }
        public DateTimeOffset ResponseDateTime { get; set; }
    }
}
