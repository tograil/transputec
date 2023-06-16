using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentTimeline
{
    public class GetIncidentTimelineRequest:IRequest<GetIncidentTimelineResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
