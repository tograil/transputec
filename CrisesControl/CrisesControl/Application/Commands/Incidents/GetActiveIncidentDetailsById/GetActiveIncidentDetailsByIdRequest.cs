using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentDetailsById
{
    public class GetActiveIncidentDetailsByIdRequest:IRequest<GetActiveIncidentDetailsByIdResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
