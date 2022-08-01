using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentBasic
{
    public class GetActiveIncidentBasicRequest:IRequest<GetActiveIncidentBasicResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
