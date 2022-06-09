using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CreateSampleIncident
{
    public class CreateSampleIncidentRequest:IRequest<CreateSampleIncidentResponse>
    {
        public int CompanyId { get; set; }
    }
}
