using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioVerifyCheck
{
    public class TwilioVerifyCheckRequest:IRequest<TwilioVerifyCheckResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
