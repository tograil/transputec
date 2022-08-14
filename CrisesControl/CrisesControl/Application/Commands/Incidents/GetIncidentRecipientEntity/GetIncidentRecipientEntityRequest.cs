using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentRecipientEntity
{
    public class GetIncidentRecipientEntityRequest:IRequest<GetIncidentRecipientEntityResponse>
    {
        public int ActiveIncidentID { get; set; }
        public string EntityType { get; set; }
        public int EntityID { get; set; }
    }
}
