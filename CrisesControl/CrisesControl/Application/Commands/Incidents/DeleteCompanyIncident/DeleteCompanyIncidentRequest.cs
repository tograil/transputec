using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncident
{
    public class DeleteCompanyIncidentRequest:IRequest<DeleteCompanyIncidentResponse>
    {
        public int IncidentId { get; set; }
    }
}
