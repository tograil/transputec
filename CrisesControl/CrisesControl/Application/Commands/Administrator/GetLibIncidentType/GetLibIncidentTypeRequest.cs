using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType
{
    public class GetLibIncidentTypeRequest:IRequest<GetLibIncidentTypeResponse>
    {
        public int LibIncidentTypeId { get; set; }
    }
}
