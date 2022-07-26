using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentTaskNotes
{
    public class GetIncidentTaskNotesRequest:IRequest<GetIncidentTaskNotesResponse>
    {
        public int ObjectID { get; set; }        
        public string NoteType { get; set; }
        public string AttachmentType { get; set; }
        public bool GetAttachments { get; set; }
    }
}
