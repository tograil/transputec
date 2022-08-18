using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelCallResponse
{
    public class HandelCallResponseRequest:IRequest<HandelCallResponseResponse>
    {
        public string CallSid { get; set; }
        public string CallStatus { get; set; }       
        public string From { get; set; }
        public string To { get; set; }
        public int CallDuration { get; set; }

    }
}
