using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class GetAllUserDevices
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserFullName UserName { get; set; }
        public int UserDeviceID { get; set; }
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
