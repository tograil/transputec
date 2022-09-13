using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioTextLog
{
    public class TwilioTextLogRequest:IRequest<TwilioTextLogResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
