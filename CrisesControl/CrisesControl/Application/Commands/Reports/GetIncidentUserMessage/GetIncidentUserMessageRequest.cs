using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentUserMessage
{
    public class GetIncidentUserMessageRequest:IRequest<GetIncidentUserMessageResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
