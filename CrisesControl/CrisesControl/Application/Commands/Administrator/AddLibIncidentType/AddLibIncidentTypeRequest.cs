using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType
{
    public class AddLibIncidentTypeRequest:IRequest<AddLibIncidentTypeResponse>
    {
        public string Name { get; set; }
        public int LibIncidentTypeId { get; set; }
    }
}
