using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetLibIncident
{
    public class GetLibIncidentRequest:IRequest<GetLibIncidentResponse>
    {
        public int LibIncidentId { get; set; }
    }
}
