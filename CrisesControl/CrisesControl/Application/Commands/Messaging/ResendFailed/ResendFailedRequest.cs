using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ResendFailed
{
    public class ResendFailedRequest: IRequest<ResendFailedResponse>
    {
        public int messageId { get; set; }
        public string commsMethod { get; set; }
    }
}
