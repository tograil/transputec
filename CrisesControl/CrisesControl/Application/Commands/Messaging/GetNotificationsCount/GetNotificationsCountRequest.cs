using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount {
    public class GetNotificationsCountRequest : IRequest<GetNotificationsCountResponse> {
        public int CurrentUserId { get; set; }
    }
}
