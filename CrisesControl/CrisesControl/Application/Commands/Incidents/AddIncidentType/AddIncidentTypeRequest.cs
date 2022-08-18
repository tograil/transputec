using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddIncidentType
{
    public class AddIncidentTypeRequest:IRequest<AddIncidentTypeResponse>
    {
        public int LibIncidentTypeId { get; set; }
        public int IncidentTypeId { get; set; }
        public string Name { get; set; }
    }
}
