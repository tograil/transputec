using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioCallAck
{
    public class TwilioCallAckRequest:IRequest<TwilioCallAckResponse>
    {
        public HttpContext context { get; set; }
    }
}
