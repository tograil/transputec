using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelTwoFactor
{
    public class HandelTwoFactorRequest:IRequest<HandelTwoFactorResponse>
    {
        public string MessageSid { get; set; }
        public SmsStatus SmsStatus { get; set; }
    }
}
