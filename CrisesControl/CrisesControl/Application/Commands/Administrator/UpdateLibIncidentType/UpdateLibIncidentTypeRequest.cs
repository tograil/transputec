using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncidentType
{
    public class UpdateLibIncidentTypeRequest:IRequest<UpdateLibIncidentTypeResponse>
    {
        public string Name { get; set; }
        public int LibIncidentTypeId { get; set; }
    }
}
