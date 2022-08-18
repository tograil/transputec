using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddIncidentAction
{
    public class AddIncidentActionRequest:IRequest<AddIncidentActionResponse>
    {
        public int IncidentId { get; set; }
        public string Title { get; set; }
        public string ActionDescription { get; set; }
        public int Status { get; set; }
        public IncidentNotificationObjLst[] IncidentParticipants { get; set; }
        public int[] UsersToNotify { get; set; }
    
    }
}
