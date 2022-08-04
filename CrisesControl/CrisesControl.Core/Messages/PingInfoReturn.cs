using CrisesControl.Core.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class PingInfoReturn
    {
        public string MessageText { get; set; }
        public int Priority { get; set; }
        public int AssetId { get; set; }
        public bool MultiResponse { get; set; }
        public bool TrackUser { get; set; }
        public bool SilentMessage { get; set; }
        public int AttachmentCount { get; set; }
        public int CascadePlanID { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public List<MediaAttachment> MessageAttachments { get; set; }
        public List<CommsMethods> MessageMethod { get; set; }
        public List<IIncNotificationLst> PingNotificationList { get; set; }
        public List<IIncNotificationLst> DepartmentToNotify { get; set; }
        public List<IIncKeyConResponse> UsersToNotify { get; set; }
    }
}
