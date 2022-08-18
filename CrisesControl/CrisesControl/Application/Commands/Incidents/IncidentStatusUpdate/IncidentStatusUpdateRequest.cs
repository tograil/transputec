using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.IncidentStatusUpdate
{
    public class IncidentStatusUpdateRequest:IRequest<IncidentStatusUpdateResponse>
    {
        public int IncidentActivationId { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; }
        public string UserRole { get; set; }
        public int NumberOfKeyHolder { get; set; }
        public string CompletionNotes { get; set; }
        public int[] MessageMethod { get; set; }
        public int CascadePlanID { get; set; }
    }
}
