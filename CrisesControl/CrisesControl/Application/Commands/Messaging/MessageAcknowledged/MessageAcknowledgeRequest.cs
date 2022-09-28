using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.MessageAcknowledged
{
    public class MessageAcknowledgeRequest:IRequest<MessageAcknowledgeResponse>
    {
        public int MsgListId { get; set; }
        public int ResponseID { get; set; }
        public string? AckMethod { get; set; }
        public string? UserLocationLong { get; set; }
        public string? UserLocationLat { get; set; }

    }
    
   
}
