using CrisesControl.Core.Messages;

namespace CrisesControl.Api.Application.Commands.Messaging.GetNotifications
{
    public class GetNotificationsResponse
    {
        public List<NotificationDetails> Result { get; set; }
    }
}
