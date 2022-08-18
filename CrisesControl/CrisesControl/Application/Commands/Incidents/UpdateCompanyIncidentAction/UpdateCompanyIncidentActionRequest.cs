using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateCompanyIncidentAction
{
    public class UpdateCompanyIncidentActionRequest:IRequest<UpdateCompanyIncidentActionResponse>
    {
        public int IncidentId { get; set; }
        public int IncidentActionId { get; set; }
        public string Title { get; set; }
        public string ActionDescription { get; set; }
        public int Status { get; set; }
        public IncidentNotificationObjLst[] IncidentParticipants { get; set; }
        public int[] UsersToNotify { get; set; }
        
    }
}
