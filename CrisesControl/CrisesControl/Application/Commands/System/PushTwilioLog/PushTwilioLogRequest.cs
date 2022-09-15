using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.PushTwilioLog
{
    public class PushTwilioLogRequest:IRequest<PushTwilioLogResponse>
    {
        public MessageType MessageType { get; set; }
        public string Sid { get; set; }
    }
}
