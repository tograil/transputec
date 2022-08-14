using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncidentAction
{
    public class DeleteCompanyIncidentActionRequest:IRequest<DeleteCompanyIncidentActionResponse>
    {
        public int IncidentActionId { get; set; }
        public int IncidentId { get; set; }
    }
}
