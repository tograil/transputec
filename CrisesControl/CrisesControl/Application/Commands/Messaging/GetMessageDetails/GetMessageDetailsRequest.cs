using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageDetails
{
    public class GetMessageDetailsRequest: IRequest<GetMessageDetailsResponse>
    {
        public string CloudMsgId { get; set; }
        public int MessageId { get; set; }
    }
}
