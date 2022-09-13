using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelSMSResponse
{
    public class HandelSMSResponseRequest:IRequest<HandelSMSResponse>
    {
        public string MessageSid { get; set; }
        public SmsStatus SmsStatus { get; set; }
    }
}
