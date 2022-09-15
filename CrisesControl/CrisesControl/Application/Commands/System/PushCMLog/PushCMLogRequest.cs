using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.PushCMLog
{
    public class PushCMLogRequest:IRequest<PushCMLogResponse>
    {
        public MessageType MessageType { get; set; }
        public string Sid { get; set; }
    }
}
