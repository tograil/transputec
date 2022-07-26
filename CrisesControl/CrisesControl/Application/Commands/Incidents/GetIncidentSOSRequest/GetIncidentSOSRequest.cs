using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentSOSRequest
{
    public class GetIncidentSOSRequest:IRequest<GetIncidentSOSRequestResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
