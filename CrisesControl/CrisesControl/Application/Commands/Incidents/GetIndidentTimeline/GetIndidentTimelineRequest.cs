using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIndidentTimeline
{
    public class GetIndidentTimelineRequest:IRequest<GetIndidentTimelineResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
