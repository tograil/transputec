using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList
{
    public class GetAllOneUserDeviceListResponse
    {
        public int UserID { get; set; }
        public UserFullName UserName { get; set; }
        public int UserDeviceID { get; set; }
        public int CompanyId { get; set; }
        public string DeviceID { get; set; }
        public string DeviceType { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceModel { get; set; }
        public string ExtraInfo { get; set; }
        public int Status { get; set; }
        public string DeviceSerial { get; set; }
        public bool SirenON { get; set; }
        public bool OverrideSilent { get; set; }
        public DateTimeOffset LastLoginFrom { get; set; }
    }
}
