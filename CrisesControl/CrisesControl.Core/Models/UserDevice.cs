using System;

namespace CrisesControl.Core.Models
{
    public partial class UserDevice
    {
        public int UserDeviceId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string DeviceId { get; set; } = null!;
        public string DeviceType { get; set; } = null!;
        public string? DeviceOs { get; set; }
        public string? DeviceModel { get; set; }
        public string? ExtraInfo { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string DeviceSerial { get; set; } = null!;
        public bool SirenOn { get; set; }
        public bool OverrideSilent { get; set; }
        public string? DeviceToken { get; set; }
        public string? SoundFile { get; set; }
    }
}
