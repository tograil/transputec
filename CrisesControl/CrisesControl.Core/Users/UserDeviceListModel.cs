using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class UserDeviceListModel
    {
        public int UserId { get; set; }
        public UserFullName UserName { get; set; }
        public int UserDeviceID { get; set; }
        public int CompanyID { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceOs { get; set; }
        public string DeviceModel { get; set; }
        public string ExtraInfo { get; set; }
        public int Status { get; set; }
        public string DeviceSerial { get; set; }
        public bool SirenOn { get; set; }
        public bool OverrideSilent { get; set; }
        public DateTimeOffset LastLoginFrom { get; set; }
}
}
