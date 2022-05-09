using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessages {
    public class GetMessagesRequest : IRequest<GetMessagesResponse> {
        public string? MessageType { get; set; }
        public int IncidentActivationId { get; set; }
        public int TargetUserId { get; set; }
    }
}
