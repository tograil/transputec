using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddNotes
{
    public class AddNotesRequest:IRequest<AddNotesResponse>
    {
        public int ActiveIncidentID { get; set; }
        public string Note { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}
