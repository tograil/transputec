using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.PingMessage
{
    public class PingMessageRequest: IRequest<PingMessageResponse>
    {
    }
}
