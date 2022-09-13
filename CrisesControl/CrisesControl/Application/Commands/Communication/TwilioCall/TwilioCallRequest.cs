using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioCall
{
    public class TwilioCallRequest:IRequest<TwilioCallResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
