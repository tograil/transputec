using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.SaveIncidentParticipants
{
    public class SaveIncidentParticipantsRequest:IRequest<SaveIncidentParticipantsResponse>
    {
        public int IncidentId { get; set; }
        public int IncidentActionId { get; set; }
        public IncidentNotificationObjLst[] IncidentParticipants { get; set; }
        public int[] UsersToNotify { get; set; }
    }
}
