using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Models
{
    public class LoginInfo
    {
        public string PushDeviceId { get; set; }
        public string DeviceSerial { get; set; }
        public string DeviceType { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceModel { get; set; }
    }

    public class LoginInfoReturnModel
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string CustomerId { get; set; }
        public string Portal { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Primary_Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanyMasterPlan { get; set; }
        public string CompanyLoginLogo { get; set; }
        public string iOSLogo { get; set; }
        public string AndroidLogo { get; set; }
        public string WindowsLogo { get; set; }
        public int UserDeviceID { get; set; }
        public string Token { get; set; }
        public string UniqueGuiID { get; set; }
        public int UserStatus { get; set; }
        public int DeviceStatus { get; set; }
        public string SupportPhone { get; set; }
        public string SupportEmail { get; set; }
        public string IncidentSiren { get; set; }
        public string PingSiren { get; set; }
        public bool OverrideSilent { get; set; }
        public bool SirenOn { get; set; }
        public string SoundFile { get; set; }
        public string FBPage { get; set; }
        public string TwitterPage { get; set; }
        public string LinkedInPage { get; set; }
        public string AppVersion { get; set; }
        public string ForceUpdate { get; set; }
        public bool FirstLogin { get; set; }
        public string UserRole { get; set; }
        public string UserLanguage { get; set; }
        public DateTimeOffset TrackingStartTime { get; set; }
        public DateTimeOffset TrackingEndTime { get; set; }
        public string TrackingInterval { get; set; }
        public bool DevMode { get; set; }
        public string DevAPI { get; set; }
        public string MessageLength { get; set; }
        public int ActiveOffDuty { get; set; }
        public string AudioRecordMaxDuration { get; set; }
        public string UniqueKey { get; set; }
        public bool TwoFactorLogin { get; set; }
        public string UserPhoto { get; set; }
        public SecItemModel? SecItems { get; set; }
        public string UploadPath { get; set; }
    }

    public class SecItemModel
    {
        public string SecurityKey { get; set; }
        public string HasAccess { get; set; }
    }
}
