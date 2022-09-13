using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioText
{
    public class TwilioTextRequest:IRequest<TwilioTextResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
