using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelConfResponse
{
    public class HandelConfResponseRequest:IRequest<HandelConfResponse>
    {
        public string CallSid { get; set; }
        public string ConferenceSid { get; set; }
        public string StatusCallbackEvent { get; set; }
      
    }
}
