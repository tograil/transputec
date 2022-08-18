using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateSOSIncident
{
    public class UpdateSOSIncidentRequest:IRequest<UpdateSOSIncidentResponse>
    {
        UpdateSOSIncidentRequest()
        {
            CascadePlanID = 0;
        }

       
        public int IncidentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IncidentTypeId { get; set; }
        public int Severity { get; set; }
       
        public string IncidentIcon { get; set; }
        public int PlanAssetID { get; set; }
        public int Status { get; set; }
        public int[] ImpactedLocation { get; set; }
        public IncidentNotificationObjLst[] NotificationGroup { get; set; }
        public UpdIncidentKeyHldLst[] UpdIncidentKeyHldLst { get; set; }
         public int NumberOfKeyHolders { get; set; }
        public int AudioAssetId { get; set; }
        public bool TrackUser { get; set; }
        public bool SilentMessage { get; set; }
        public List<AckOption> AckOptions { get; set; }
        public int[] MessageMethod { get; set; }
        public int CascadePlanID { get; set; }
        public int[] Groups { get; set; }
        public int[] IncidentKeyholder { get; set; }
    }
}
