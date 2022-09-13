using CrisesControl.Core.Communication;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioRecordingLog
{
    public class TwilioRecordingLogRequest:IRequest<TwilioRecordingLogResponse>
    {
        public TwilioRequest Model { get; set; }
    }
}
