using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse {
    public class GetMessageResponseRequest : IRequest<GetMessageResponseResponse> {
        public int ResponseID { get; set; }
        public string MessageType { get; set; }
    }
}
