using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelUnifonicCallResponse
{
    public class HandelUnifonicCallResponseRequest:IRequest<HandelUnifonicCallResponse>
    {
        public string callSid { get; set; }
        public string referenceId { get; set; }
        public int duration { get; set; }
        public string status { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public double timestamp { get; set; }
    }
}
