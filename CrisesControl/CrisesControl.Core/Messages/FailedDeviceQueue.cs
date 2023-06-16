using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class FailedDeviceQueue
    {
        public int? MessageDeviceId { get; set; }
        public string? Method { get; set; }
        public int? MessageId { get; set; }
        public int? MessageListId { get; set; }
        public string? DeviceAddress { get; set; }
        public string? MessageText { get; set; }
        public string? LockStatus { get; set; }
        public int? Attempt { get; set; }
        public string? Status { get; set; }
        public string? MessageType { get; set; }
        public DateTimeOffset? CreatedTimeZone { get; set; }
        public bool IsTaskRecepient { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? CompanyId { get; set; }
        public int? IncidentActivationId { get; set; }
        public string? DeviceType { get; set; }
        public bool? TrackUser { get; set; }
        public int? CreatedBy { get; set; }
        public int? UserId { get; set; }
        public string? ISDCode { get; set; }
        //public string MobileNo { get; set; }
        public bool MultiResponse { get; set; }
        public string? Company_Name { get; set; }
        public string? CompanyLogoPath { get; set; }
        //public string TaskURL { get; set; }

        public bool SirenON { get; set; }
        public bool OverrideSilent { get; set; }
        public string? SoundFile { get; set; }
        public bool SilentMessage { get; set; }
        public string? SenderFirstName { get; set; }
        public string? SenderLastName { get; set; }                        
  
    }
}
