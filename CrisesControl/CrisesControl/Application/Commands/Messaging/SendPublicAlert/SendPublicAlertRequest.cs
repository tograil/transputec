using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.SendPublicAlert
{
    public class SendPublicAlertRequest : IRequest<SendPublicAlertResponse>
    {
    }
}
