using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateSOS
{
    public class UpdateSOSRequest:IRequest<UpdateSOSResponse>
    {
        public int ActiveIncidentID { get; set; }
        public int SOSAlertID { get; set; }
        public int MessageListID { get; set; }
        public int UserID { get; set; }
        public string SOSClosureNotes { get; set; }
        public bool CloseSOS { get; set; }
        public bool CloseAllSOS { get; set; }
        public int[] CaseNoteIDs { get; set; }
        public bool MultiNotes { get; set; }
        public bool CloseSOSIncident { get; set; }
    }
}
