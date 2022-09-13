using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioCallLog
{
    public class TwilioCallLogRequest:IRequest<TwilioCallLogResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
