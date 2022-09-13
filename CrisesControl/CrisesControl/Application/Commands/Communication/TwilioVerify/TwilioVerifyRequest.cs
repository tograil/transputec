using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioVerify
{
    public class TwilioVerifyRequest:IRequest<TwilioVerifyResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
