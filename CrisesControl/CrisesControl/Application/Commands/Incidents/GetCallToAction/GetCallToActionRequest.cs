using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCallToAction
{
    public class GetCallToActionRequest:IRequest<GetCallToActionResponse>
    {
        public int ActiveIncidentId { get; set; }
    }
}
