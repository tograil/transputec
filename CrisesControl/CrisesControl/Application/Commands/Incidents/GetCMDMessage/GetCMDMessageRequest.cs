using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCMDMessage
{
    public class GetCMDMessageRequest:IRequest<GetCMDMessageResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
