using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentByName
{
    public class GetIncidentByNameRequest:IRequest<GetIncidentByNameResponse>
    {
        public string IncidentName { get; set; }
    }
}
