using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioEndConferenceCall
{
    public class TwilioEndConferenceCallRequest:IRequest<TwilioEndConferenceCallResponse>
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
       
        public string Sid { get; set; }
        
        public string DataCenter { get; set; }
    }
}
