using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class PingMessageQuery
    {
        public int CompanyId { get; set; }
        public string? MessageText { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public int Priority { get; set; }
        public bool MultiResponse { get; set; }
        public string MessageType { get; set; }

        public int IncidentActivationId { get; set; }
        public int CurrentUserId { get; set; }
        public string TimeZoneId { get; set; }
        public PingMessageObjLst[] PingMessageObjLst { get; set; }
        public int[] UsersToNotify { get; set; }
        public int AudioAssetId { get; set; } = 0;
        public bool SilentMessage { get; set; } = false;
        public int[] MessageMethod { get; set; }
        public List<MediaAttachment> MediaAttachments { get; set; }
        public List<string> SocialHandle { get; set; }
        public int CascadePlanID { get; set; } = 0;
        public bool SendToAllRecipient { get; set; }
    }
}
