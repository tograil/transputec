using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioConfLog
{
    public class TwilioConfLogRequest:IRequest<TwilioConfLogResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
