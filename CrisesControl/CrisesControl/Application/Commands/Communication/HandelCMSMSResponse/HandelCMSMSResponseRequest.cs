using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelCMSMSResponse
{
    public class HandelCMSMSResponseRequest:IRequest<HandelCMSMSResponse>
    {
        public string MessageSid { get; set; }
        public string Status { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
      
    }
}
