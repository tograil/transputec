using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.PingMessage
{
    public class PingMessageRequest: IRequest<PingMessageResponse>
    {
      
        public string? MessageText { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public int Priority { get; set; }
        public bool MultiResponse { get; set; }
        public string MessageType { get; set; }
        public int IncidentActivationId { get; set; }
        public PingMessageObjLst[] PingMessageObjLst { get; set; }
        public int[] UsersToNotify { get; set; }
        public int AssetId { get; set; } = 0;
        public bool SilentMessage { get; set; } = false;
        public int[] MessageMethod { get; set; }
        public List<MediaAttachment> MediaAttachments { get; set; }
        public List<string> SocialHandle { get; set; }
        public int CascadePlanID { get; set; } = 0;
    }
}
