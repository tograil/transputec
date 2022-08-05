using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ResendFailed
{
    public class ResendFailedRequest: IRequest<ResendFailedResponse>
    {
    }
}
