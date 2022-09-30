using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident
{
    public class GetAllCompanyIncidentRequest:IRequest<GetAllCompanyIncidentResponse>
    {
        public int UserId { get; set; }
    }
}
