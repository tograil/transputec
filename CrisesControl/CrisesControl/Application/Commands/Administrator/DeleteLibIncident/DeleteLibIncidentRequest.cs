using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident
{
    public class DeleteLibIncidentRequest:IRequest<DeleteLibIncidentResponse>
    {
        public int LibIncidentId { get; set; }
      
  
    }
}
