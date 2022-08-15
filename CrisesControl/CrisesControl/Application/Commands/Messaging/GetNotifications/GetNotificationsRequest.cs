using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetNotifications
{
    public class GetNotificationsRequest: IRequest<GetNotificationsResponse>
    {
        public int CompanyId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
