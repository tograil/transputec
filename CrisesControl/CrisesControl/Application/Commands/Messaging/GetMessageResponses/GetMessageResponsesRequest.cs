using MediatR;
using System.Runtime.Serialization;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses {
    public class GetMessageResponsesRequest : IRequest<GetMessageResponsesResponse> {
        public string MessageType { get; set; }
        public int Status { get; set; }
    }
}
