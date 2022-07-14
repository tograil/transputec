using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType
{
    public class DeleteLibIncidentTypeRequest:IRequest<DeleteLibIncidentTypeResponse>
    {
        public int LibIncidentTypeId { get; set; }
    }
}
