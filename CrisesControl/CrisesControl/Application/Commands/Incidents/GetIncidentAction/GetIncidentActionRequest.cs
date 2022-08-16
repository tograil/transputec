using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentAction
{
    public class GetIncidentActionRequest:IRequest<GetIncidentActionResponse>
    {
        public int IncidentActionId { get; set; }
        public int IncidentId { get; set; }
    }
}
