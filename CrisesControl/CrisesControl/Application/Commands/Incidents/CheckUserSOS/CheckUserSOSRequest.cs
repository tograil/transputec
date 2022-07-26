using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.CheckUserSOS
{
    public class CheckUserSOSRequest:IRequest<CheckUserSOSResponse>
    {
        public int ActiveIncidentId { get; set; }
    }
}
