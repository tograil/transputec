using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMessage
{
    public class GetIncidentMessageRequest:IRequest<GetIncidentMessageResponse>
    {
        public int IncidentId { get; set; }
    }
}
